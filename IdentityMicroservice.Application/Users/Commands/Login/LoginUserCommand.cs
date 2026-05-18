namespace IdentityMicroservice.Application.Users.Commands.Login;

public record LoginUserCommand(string Email, string Password);