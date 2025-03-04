using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CustomerAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using CustomerAPI.Data;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using CustomerAPI.Controllers;
using CustomerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerAPI.Tests
{
    public class RegistrationControllerTests
    {
        private readonly Mock<IRegistrationService> _mockService;
        private readonly Mock<ILogger<RegistrationController>> _mockLogger;
        private readonly RegistrationController _controller;

        public RegistrationControllerTests()
        {
            _mockService = new Mock<IRegistrationService>();
            _mockLogger = new Mock<ILogger<RegistrationController>>();
            _controller = new RegistrationController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Register_ValidRequest_ReturnsCreatedResult()
        {
            // Arrange
            RegistrationRequest request = new RegistrationRequest
            {
                FirstName = "John",
                LastName = "Smith",
                PolicyReferenceNumber = "AB-123456",
                Email = "john.smith@example.com"
            };

            const int expectedCustomerId = 12345;
            _mockService.Setup(s => s.Register(It.IsAny<RegistrationRequest>()))
                .ReturnsAsync(expectedCustomerId);

            // Act
            ActionResult<RegistrationResponse> result = await _controller.Register(request);

            // Assert
            CreatedAtActionResult createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            RegistrationResponse response = Assert.IsType<RegistrationResponse>(createdResult.Value);
            Assert.Equal(expectedCustomerId, response.CustomerId);
            Assert.Equal("Registration successful", response.Message);
        }

        [Fact]
        public async Task Register_ValidationFails_ReturnsBadRequest()
        {
            // Arrange
            RegistrationRequest request = new RegistrationRequest
            {
                FirstName = "John",
                LastName = "Smith",
                PolicyReferenceNumber = "AB-123456",
                Email = "john.smith@example.com"
            };

            _mockService.Setup(s => s.Register(It.IsAny<RegistrationRequest>()))
                .ThrowsAsync(new ValidationException("Validation failed"));

            // Act
            ActionResult<RegistrationResponse> result = await _controller.Register(request);

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            
            //Reflection is costly but should be okay here as it's only a test
            object? error = badRequest.Value?.GetType().GetProperty("error")?.GetValue(badRequest.Value, null);

            if (error != null)
            {
                Assert.Equal("Validation failed", error.ToString());
            }
            else
            {
                Assert.Fail("Error property not found.");
            }
        }
    }

    // Integration tests with a test web server and in memory database to test controller
    public class RegistrationApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public RegistrationApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false // Prevents HTTP to HTTPS redirects in tests
            });
        }

        [Fact]
        public async Task Register_ValidRequest_ReturnsCreatedWithCustomerId()
        {
            // Arrange
            RegistrationRequest request = new RegistrationRequest
            {
                FirstName = "Jane",
                LastName = "Doe",
                PolicyReferenceNumber = "XY-987654",
                Email = "jane.doe@example.com"
            };

            // Log the payload
            string requestJson = JsonSerializer.Serialize(request);
            Console.WriteLine("Request Payload: " + requestJson);
            
            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/Registration/register", request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            RegistrationResponse? result = await response.Content.ReadFromJsonAsync<RegistrationResponse>();
            Assert.NotNull(result);
            Assert.True(result.CustomerId > 0);
            Assert.Equal("Customer registration successful", result.Message);
        }

        [Fact]
        public async Task Register_InvalidEmailFormat_ReturnsBadRequest()
        {
            // Arrange
            RegistrationRequest request = new RegistrationRequest
            {
                FirstName = "Jane",
                LastName = "Doe",
                PolicyReferenceNumber = "XY-987654",
                Email = "invalid-email" // Invalid email format
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/Registration/register", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_CustomerUnder18_ReturnsBadRequest()
        {
            // Arrange
            RegistrationRequest request = new RegistrationRequest
            {
                FirstName = "Young",
                LastName = "Person",
                PolicyReferenceNumber = "ZZ-123456",
                DateOfBirth = DateTime.Today.AddYears(-17) // 17 years old
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/Registration/register", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    // Custom WebApplicationFactory for integration tests
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Set the environment to Testing so that Program.cs knows to use the correct settings and inMemory database
            builder.UseEnvironment("Testing");
            
            // Ensure the logging is configured so we can see what's going on
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Debug); // Show all logs during tests
            });
            
            builder.ConfigureServices(services =>
            {
                // Build the service provider
                ServiceProvider sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database context
                using var scope = sp.CreateScope();
                IServiceProvider scopedServices = scope.ServiceProvider;
                CustomerDbContext db = scopedServices.GetRequiredService<CustomerDbContext>();
                ILogger<CustomWebApplicationFactory> logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory>>();

                // Ensure the database is created
                db.Database.EnsureCreated();
            });
        }
    }
}
