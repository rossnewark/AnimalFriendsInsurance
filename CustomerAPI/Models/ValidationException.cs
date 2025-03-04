namespace CustomerAPI.Models;

public class ValidationException : Exception
{
    /// <summary>
    /// Validation Exception class so that we can distinguish between a failure due to validation and a generic exception
    /// </summary>
    /// <param name="message"></param>
    public ValidationException(string message) : base(message)
    {
    }
}