using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Services
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(User user);
    }
}
