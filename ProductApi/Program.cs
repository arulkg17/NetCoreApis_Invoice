using customerApi.Services;
using Invoice.Authentication.Configurations;
using Invoice.Authentication.Extensions;
using Invoice.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using OpenAI;
using ProductApi.Contracts;
using ProductApi.Data;
using ProductApi.Middleware;
using ProductApi.Repositories;
using ProductApi.Services;
using Serilog;
using System.Data;
using System.Text;
using System.Threading.RateLimiting;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
// Kestrel timeout config — must come before Build()
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
});
builder.Host.UseSerilog();

//var openAiApiKey = builder.Configuration["OpenAI:ApiKey"];

//if (string.IsNullOrWhiteSpace(openAiApiKey))
//{
//    throw new InvalidOperationException("OpenAI API key is not configured.");
//}

//builder.Services.AddSingleton(new OpenAIClient(openAiApiKey));

builder.Services.AddAutoMapper(typeof(Program));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IVendorRepository, VendorRepositoryEFSp>();
builder.Services.AddScoped<IVendorService, VendorServiceEFSp>();
builder.Services.AddScoped<IProductSPRepository, ProductSPRepository>();
builder.Services.AddScoped<IProductSPService, ProductSPService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepositories>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IItemmasterRepository, ItemmasterRepositoryEFSp>();
builder.Services.AddScoped<IItemmasterService, ItemmasterServiceEFSp>();

var useRealAI = builder.Configuration.GetValue<bool>("UseRealAI");

if (useRealAI)
{
    builder.Services.AddScoped(sp =>
    {
        var handler = new ProductApi.AI.OllamaLoggingHandler
        {
            InnerHandler = new HttpClientHandler()
        };

        var ollamaHttpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromMinutes(10)
        };

        var kernelBuilder = Kernel.CreateBuilder();

        kernelBuilder.AddOpenAIChatCompletion(
            modelId: builder.Configuration["Ollama:Model"] ?? "qwen2.5:7b-instruct",
            endpoint: new Uri(
                builder.Configuration["Ollama:Endpoint"]
                ?? "http://localhost:11434/v1"),
            apiKey: "not-needed",
            httpClient: ollamaHttpClient);

        var kernel = kernelBuilder.Build();

        kernel.FunctionInvocationFilters.Add(
            new FunctionLoggingFilter(
                sp.GetRequiredService<
                    ILogger<FunctionLoggingFilter>>()));

        //kernel.Plugins.AddFromType<ProductApi.Plugins.CategoryPlugin>(
        //    "CategoryPlugin", sp);

        kernel.Plugins.AddFromType<ProductApi.Plugins.CustomerPlugin>(
            "CustomerPlugin", sp);

        //kernel.Plugins.AddFromType<ProductApi.Plugins.VendorPlugin>(
        //    "VendorPlugin", sp);

        //kernel.Plugins.AddFromType<ProductApi.Plugins.UserPlugin>(
        //    "UserPlugin", sp);

        //kernel.Plugins.AddFromType<ProductApi.Plugins.ItemMasterPlugin>(
        //    "ItemMasterPlugin", sp);

        return kernel;
    });

    builder.Services.AddScoped<IMockAIProvider, OllamaAIProvider>();
}
else
{
    builder.Services.AddScoped<IMockAIProvider, MockAIProvider>();
}

builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<IMultiIntentAIProvider, MockMultiIntentAIProvider>();

//builder.Services.AddScoped<IDbConnection>(sp =>
//{
//    var configuration = sp.GetRequiredService<IConfiguration>();
//    var connectionString =
//        configuration.GetConnectionString("DefaultConnection");

//    return new SqlConnection(connectionString);
//});

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var connectionString =
        configuration.GetConnectionString("DefaultConnection");

    //var sqlBuilder =
    //    new SqlConnectionStringBuilder(connectionString);

    //Log.Information(
    //    "DB CONFIG - Server: {Server}, Database: {Database}, User: {User}, Encrypt: {Encrypt}, TrustServerCertificate: {TrustServerCertificate}",
    //    sqlBuilder.DataSource,
    //    sqlBuilder.InitialCatalog,
    //    sqlBuilder.UserID,
    //    sqlBuilder.Encrypt,
    //    sqlBuilder.TrustServerCertificate);

    return new SqlConnection(connectionString);
});

builder.Services.AddScoped<IUserRepository, UserRepositorySpDap>();
builder.Services.AddScoped<IUserService, UserServiceSpDap>();

// configure EF Core with SQL Server LocalDB (change to your SQL Server if needed)
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? "Server=LAPTOP-BIG8QIRC;Database=Accounts;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(conn));

//Reference if Invoiuce.Authentication library 
//builder.Services.AddAuthenticationLibrary();
//builder.Services.Configure<JwtOptions>(
//    builder.Configuration.GetSection("Jwt")
//    );
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// configure CORS so React (localhost:3000) can call API
var AllowReact = "_allowReact";
var allowOrigin = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowReact,
        policy =>
        {
            policy.WithOrigins(allowOrigin)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var jwtKey = builder.Configuration["Jwt:Key"] ?? "v2UJQxTrwUCqqJkehkxvSUZKQCX6gNmRWq7q1bWa3Jw=";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "yourapiissuer";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; ;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)) 
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Insert JWT like: Bearer {your token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("FixedPolicy", policyOptions =>
    {
        policyOptions.PermitLimit = 100;
        policyOptions.Window = TimeSpan.FromMinutes(1);
        policyOptions.QueueLimit = 0;
        policyOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.Headers["Retry-After"] = "60"; // seconds, same as Window duration
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
    };
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "ProductAPAI.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//Console.WriteLine("Before Seagger");
    app.UseSwagger();
//Console.WriteLine("After Swagger");
    app.UseSwaggerUI();
//Console.WriteLine("After Swagger UI");
//}
app.UseRouting();
//Console.WriteLine("After Routing");
app.UseCors(AllowReact);
//Console.WriteLine("After Cors");
app.UseAuthentication();
//Console.WriteLine("After Authentication");
app.UseAuthorization();
//Console.WriteLine("After Authorization");

app.UseRateLimiter();
//Console.WriteLine("After Ratelimit");
app.UseExceptionLogging();
//Console.WriteLine("After ExceptionLogging");
app.UseCorrelationIdLogging();
//Console.WriteLine("After CorrelationIdLogging");
app.UseSession();
//Console.WriteLine("After Session");
app.UseMiddleware<ConcvurrencyExcepyionMiddleware>();
//Console.WriteLine("After ConcurrencyExceptionMiddleware");  

app.MapControllers().RequireRateLimiting("FixedPolicy");
//Console.WriteLine("After MapController");
app.Lifetime.ApplicationStarted.Register(() =>
{
    //Console.WriteLine("Application Started Successfully");

    foreach (var url in app.Urls)
    {
        //Console.WriteLine($"Listening On : {url}");
    }
});

app.Lifetime.ApplicationStopping.Register(() =>
{
    //Console.WriteLine("Application Stopping");
});

app.Run();


public sealed class OllamaLoggingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var requestBody = request.Content == null
            ? ""
            : await request.Content.ReadAsStringAsync(cancellationToken);

        Console.WriteLine("========== OLLAMA REQUEST ==========");
        Console.WriteLine($"{request.Method} {request.RequestUri}");
        Console.WriteLine(requestBody);

        var response = await base.SendAsync(request, cancellationToken);

        var responseBody = response.Content == null
            ? ""
            : await response.Content.ReadAsStringAsync(cancellationToken);

        stopwatch.Stop();

        Console.WriteLine("========== OLLAMA RESPONSE ==========");
        Console.WriteLine($"Status: {(int)response.StatusCode}");
        Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine(responseBody);
        Console.WriteLine("====================================");

        return response;
    }
}

