namespace IdentityMicroservice.Domain.Shared;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Erro inválido combinado com estado de sucesso", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }
    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error) 
        : base(isSuccess, error)
    {
        _value = value;
    }
    public TValue Value => IsSuccess 
        ? _value! 
        : throw new InvalidOperationException("Não é possível acessar o valor de um resultado de falha.");

    public static implicit operator Result<TValue>(TValue? value) => new(value, true, Error.None);
    public static implicit operator Result<TValue>(Error error) => new(default, false, error);
}