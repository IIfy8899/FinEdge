using FinEdge.Application.UserAuthentication.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinEdge.API.Controllers;

public class UserController : ApiControllerBase
{
    [AllowAnonymous]
    [HttpPost("user.register", Name = nameof(Register))]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        return result.Succeeded
            ? Ok(result)
            : BadRequest(result);
    }
}
