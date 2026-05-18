using IdentityMicroservice.Application.Interfaces;
using IdentityMicroservice.Domain.Entities;
using IdentityMicroservice.Domain.Errors;
using IdentityMicroservice.Domain.Repositories;
using IdentityMicroservice.Domain.Shared;

namespace IdentityMicroservice.Application.Users.Commands.Register;

public class RegisterUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var isEmailUnique = await _userRepository.IsEmailUniqueAsync(command.Email, cancellationToken);
        if (!isEmailUnique)
        {
            return DomainErrors.User.EmailAlreadyInUse;
        }

        var passwordHash = _passwordHasher.HashPassword(command.Password);

        var userResult = User.Create(command.Name, command.Email, passwordHash);
        if (userResult.IsFailure)
        {
            return userResult.Error;
        }

        await _userRepository.AddAsync(userResult.Value, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return userResult.Value.Id;
    }
}