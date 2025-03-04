using CustomerAPI.Data;
using CustomerAPI.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Remove any Loggers from the builder
builder.Logging.ClearProviders();

// Add console logging for visibility
builder.Logging.AddConsole(); 

// Add Debug Logging for Development and Testing environments
if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
{
    builder.Logging.AddDebug();
}

// Load environment-specific configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container
builder.Services.AddControllers();

// Check if the environment is Testing, and use the InMemory database
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<CustomerDbContext>(options => options.UseInMemoryDatabase("InMemoryDbForTesting"));
}
else
{
    // Try and get the default connection string
    string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("The connection string 'DefaultConnection' is missing or empty.");
    }
    
    // Configure SQL Server for production or development
    builder.Services.AddDbContext<CustomerDbContext>(options =>
        options.UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));
}

// Register services -> this could be broken out into a new file if the list grows
builder.Services.AddScoped<IRegistrationService, RegistrationService>();

// Configure Swagger so that we can use the Swagger webpage to test with
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Animal Friends Insurance - Customer Registration API", 
        Version = "v1",  
        Description = "API for registering customers to the AFI customer portal"
    });

    c.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return new[] {api.GroupName};
        }

        ControllerActionDescriptor? controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
        if (controllerActionDescriptor != null)
        {
            return new[] {controllerActionDescriptor.ControllerName};
        }

        throw new InvalidOperationException("Unable to determine a tag for endpoint.");
    });
});

// Build the API
WebApplication app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AFI Registration API v1");
        c.RoutePrefix = "swagger";
    });
    
    // Auto-migrate database in development
    using (var scope = app.Services.CreateScope())
    {
        CustomerDbContext db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
        db.Database.Migrate();
    }
}

// If we're NOT testing we require HTTPS redirection so turn it on.
if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.MapControllers();
app.Run();

public partial class Program { }