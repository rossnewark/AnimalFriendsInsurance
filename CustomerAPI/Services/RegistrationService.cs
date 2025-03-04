using CustomerAPI.Data;
using CustomerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerAPI.Services;

public class RegistrationService : IRegistrationService
{
    private readonly CustomerDbContext _dbContext;
    private readonly ILogger<RegistrationService> _logger;
        
    public RegistrationService(CustomerDbContext dbContext, ILogger<RegistrationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
        
    public async Task<int> Register(RegistrationRequest request)
    {
        // Validate the request
        await ValidateRequest(request);
            
        // Create a customer entity
        Customer customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PolicyReferenceNumber = request.PolicyReferenceNumber,
            DateOfBirth = request.DateOfBirth,
            Email = request.Email,
            RegistrationDate = DateTime.UtcNow
        };
            
        // Save to database
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();
            
        _logger.LogInformation("Customer registration successful. CustomerId: {CustomerId}", customer.CustomerId);
            
        return customer.CustomerId;
    }
        
    private async Task ValidateRequest(RegistrationRequest request)
    {
        // Check for an existing policy reference
        Customer? existingCustomer = await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.PolicyReferenceNumber == request.PolicyReferenceNumber);
            
        if (existingCustomer != null)
        {
            throw new ValidationException($"A customer with policy reference {request.PolicyReferenceNumber} is already registered");
        }
            
        // Check if either DOB or Email is provided
        if (!request.DateOfBirth.HasValue && string.IsNullOrEmpty(request.Email))
        {
            throw new ValidationException("Either Date of Birth or Email must be provided");
        }
            
        // Validate age if DOB is provided
        if (request.DateOfBirth.HasValue)
        {
            int age = DateTime.Today.Year - request.DateOfBirth.Value.Year;
                
            // Adjust age if birthday hasn't occurred yet this year
            if (request.DateOfBirth.Value.Date > DateTime.Today.AddYears(-age))
            {
                age--;
            }
                
            if (age < 18)
            {
                throw new ValidationException("Customer must be at least 18 years old");
            }
        }
    }
}