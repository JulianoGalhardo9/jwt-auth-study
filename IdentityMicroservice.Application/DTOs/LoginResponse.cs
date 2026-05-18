namespace IdentityMicroservice.Application.DTOs;
public record LoginResponse(string AccessToken, string RefreshToken);