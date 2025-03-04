using System.ComponentModel.DataAnnotations;

namespace CustomerAPI.Models
{
    public class RegistrationRequest
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "First name must be between 3 and 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Last name must be between 3 and 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Policy reference number is required")]
        [RegularExpression(@"^[A-Z]{2}-\d{6}$",
            ErrorMessage = "Policy reference must be in format XX-999999 (two capital letters, hyphen, six digits)")]
        public string PolicyReferenceNumber { get; set; }

        // One of Date of Birth or Email is required, handled by custom validation
        public DateTime? DateOfBirth { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]{4,}@[a-zA-Z0-9]{2,}(\.(com|co\.uk))$",
            ErrorMessage = "An email address must contain at least 4 characters, followed by an '@', " +
                           "at least 2 more characters, and end with '.com' or '.co.uk'")]
        public string Email { get; set; }
    }
}