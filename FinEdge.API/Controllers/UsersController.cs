using FinEdge.Application.UserAuthentication.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinEdge.API.Controllers;

[AllowAnonymous]
[Route("api/users/")]
public class UserController : ApiControllerBase
{
    [HttpPost("register", Name = nameof(Register))]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        return result.Succeeded
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("login", Name = nameof(Login))]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        return result.Succeeded
            ? Ok(result)
            : Unauthorized(result);
    }
}
