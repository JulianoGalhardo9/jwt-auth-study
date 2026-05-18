namespace IdentityMicroservice.Application.Users.Commands.Register;
public record RegisterUserCommand(string Name, string Email, string Password);