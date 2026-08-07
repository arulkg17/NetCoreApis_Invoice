using ProductApi.Contracts;
using ProductApi.Repositories;
using ProductApi.Services;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using ProductApi.Middleware;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using customerApi.Services;
using Invoice.Authentication.Extensions;
using Invoice.Models;
using Invoice.Authentication.Configurations;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddAutoMapper(typeof(Program));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<IProductSPRepository, ProductSPRepository>();
builder.Services.AddScoped<IProductSPService, ProductSPService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepositories>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IItemmasterRepository, ItemmasterRepository>();
builder.Services.AddScoped<IItemmasterService, ItemmasterService>();

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
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowReact,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:4200", 
                "http://localhost:5173")
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
