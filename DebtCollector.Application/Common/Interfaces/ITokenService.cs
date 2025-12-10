using DebtCollector.Domain.Entities;

namespace DebtCollector.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(Person person);
        string GenerateRefreshToken();
        Task<string> GenerateUniqueRandomPasswordAsync(int length);
    }
}
