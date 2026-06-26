using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load Ocelot config
builder.Configuration.AddJsonFile("ocelot.json");

// Add Ocelot
builder.Services.AddOcelot();

var app = builder.Build();

app.MapGet("/", () => "API Gateway Running");

await app.UseOcelot();

app.Run();