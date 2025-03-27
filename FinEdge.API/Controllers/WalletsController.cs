using FinEdge.Application.UserTransaction.Commands;
using FinEdge.Application.UserWallet.Commands;
using FinEdge.Application.UserWallet.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinEdge.API.Controllers;

[Route("api/wallets/")]
public class WalletsController : ApiControllerBase
{
    [HttpPost(Name = nameof(CreateWallet))]
    public async Task<IActionResult> CreateWallet(
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var command = new CreateWalletCommand(userId);
        var result = await Mediator.Send(command, cancellationToken);

        return result.Succeeded
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpGet("{id}", Name = nameof(GetWallet))]
    public async Task<IActionResult> GetWallet(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var query = new GetWalletQuery(id, userId);
        var result = await Mediator.Send(query, cancellationToken);

        return result.Succeeded
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("{walletId}/fund", Name = nameof(FundWallet))]
    public async Task<IActionResult> FundWallet(
        int walletId,
        [FromBody] FundWalletCommand command,
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        command.WalletId = walletId;
        command.UserId = userId;

        var result = await Mediator.Send(command, cancellationToken);

        return result.Succeeded
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("{walletId}/transactions", Name = nameof(CreateTransaction))]
    public async Task<IActionResult> CreateTransaction(
        int walletId,
        [FromBody] CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        command.WalletId = walletId;
        command.UserId = userId;
        var result = await Mediator.Send(command, cancellationToken);

        return result.Succeeded
            ? Ok(result)
            : BadRequest(result);
    }
}
