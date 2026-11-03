using PawCareApi.Domain.Entities;

namespace PawCareApi.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        (string Token, DateTime Expiration) GenerateToken(User user);
    }
}

