using IdentityMicroservice.Domain.Entities;

namespace IdentityMicroservice.Domain.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}