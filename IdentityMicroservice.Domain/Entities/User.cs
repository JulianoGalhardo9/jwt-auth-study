using IdentityMicroservice.Domain.Shared;

namespace IdentityMicroservice.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    private User() { }

    public static Result<User> Create(string name, string email, string passwordHash)
{
    if (string.IsNullOrWhiteSpace(name))
        return new Error("User.NameRequired", "O nome é obrigatório.");
        
    if (string.IsNullOrWhiteSpace(email))
        return new Error("User.EmailRequired", "O e-mail é obrigatório.");

    var user = new User
    {
        Id = Guid.NewGuid(),
        Name = name,
        Email = email,
        PasswordHash = passwordHash
    };

    return user; 
}
    public void UpdateRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
    }
    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
    }
}