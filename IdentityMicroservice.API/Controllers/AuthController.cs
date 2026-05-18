using FluentValidation;
using IdentityMicroservice.Application.DTOs;
using IdentityMicroservice.Application.Users.Commands.Login;
using IdentityMicroservice.Application.Users.Commands.Register;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroservice.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserCommandHandler _registerHandler;
    private readonly LoginUserCommandHandler _loginHandler;

    public AuthController(
        RegisterUserCommandHandler registerHandler,
        LoginUserCommandHandler loginHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        [FromServices] IValidator<RegisterUserCommand> validator,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.Name, request.Email, request.Password);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }

        var result = await _registerHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Code, description = result.Error.Description });
        }

        return CreatedAtAction(nameof(Register), new { id = result.Value }, new { userId = result.Value });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        [FromServices] IValidator<LoginUserCommand> validator,
        CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request.Email, request.Password);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }

        var result = await _loginHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(new { error = result.Error.Code, description = result.Error.Description });
        }

        return Ok(result.Value);
    }
}