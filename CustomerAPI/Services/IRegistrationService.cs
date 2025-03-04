using CustomerAPI.Models;

namespace CustomerAPI.Services
{
    public interface IRegistrationService
    {
        Task<int> Register(RegistrationRequest request);
    }
}