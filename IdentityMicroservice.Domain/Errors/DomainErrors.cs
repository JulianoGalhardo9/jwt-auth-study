using IdentityMicroservice.Domain.Shared;

namespace IdentityMicroservice.Domain.Errors;

public static class DomainErrors
{
    public static class User
    {
        public static readonly Error EmailAlreadyInUse = new(
            "User.EmailAlreadyInUse", "O e-mail fornecido já está em uso.");

        public static readonly Error InvalidCredentials = new(
            "User.InvalidCredentials", "E-mail ou senha incorretos.");

        public static readonly Error InvalidToken = new(
            "User.InvalidToken", "O Refresh Token fornecido é inválido.");

        public static readonly Error TokenExpired = new(
            "User.TokenExpired", "O Refresh Token fornecido já expirou. Faça login novamente.");
    }
}