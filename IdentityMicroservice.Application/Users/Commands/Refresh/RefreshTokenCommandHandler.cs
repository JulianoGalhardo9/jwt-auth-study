using IdentityMicroservice.Application.DTOs;
using IdentityMicroservice.Application.Interfaces;
using IdentityMicroservice.Domain.Errors;
using IdentityMicroservice.Domain.Repositories;
using IdentityMicroservice.Domain.Services;
using IdentityMicroservice.Domain.Shared;

namespace IdentityMicroservice.Application.Users.Commands.Refresh;

public class RefreshTokenCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(command.RefreshToken, cancellationToken);
        if (user is null)
        {
            return DomainErrors.User.InvalidToken;
        }

        if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            user.RevokeRefreshToken();
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return DomainErrors.User.TokenExpired;
        }

        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.UpdateRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(newAccessToken, newRefreshToken);
    }
}