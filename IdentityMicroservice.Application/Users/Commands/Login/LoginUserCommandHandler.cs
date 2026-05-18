using IdentityMicroservice.Application.DTOs;
using IdentityMicroservice.Application.Interfaces;
using IdentityMicroservice.Domain.Errors;
using IdentityMicroservice.Domain.Repositories;
using IdentityMicroservice.Domain.Services;
using IdentityMicroservice.Domain.Shared;

namespace IdentityMicroservice.Application.Users.Commands.Login;

public class LoginUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse>> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (user is null)
        {
            return DomainErrors.User.InvalidCredentials;
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(command.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return DomainErrors.User.InvalidCredentials;
        }

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(accessToken, refreshToken);
    }
}