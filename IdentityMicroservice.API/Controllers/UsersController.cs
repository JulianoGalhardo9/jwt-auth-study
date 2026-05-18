using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroservice.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        return Ok(new { message = "Você está autenticado! Acesso concedido ao domínio seguro.", timestamp = DateTime.UtcNow });
    }
}