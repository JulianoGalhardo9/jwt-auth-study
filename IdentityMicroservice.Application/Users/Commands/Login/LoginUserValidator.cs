using FluentValidation;

namespace IdentityMicroservice.Application.Users.Commands.Login;

public class LoginUserValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Formato de e-mail inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.");
    }
}