namespace CustomerAPI.Data;

public class Customer
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PolicyReferenceNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Email { get; set; }
    public DateTime RegistrationDate { get; set; }
}