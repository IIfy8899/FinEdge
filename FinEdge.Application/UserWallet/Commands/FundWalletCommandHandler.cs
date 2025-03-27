using FinEdge.Application.Common.Interfaces;
using FinEdge.Domain.Entities;
using UnauthorizedAccessException = FinEdge.Application.Exceptions.UnauthorizedAccessException;
using NotFoundException = FinEdge.Application.Exceptions.NotFoundException;
using MediatR;
using System.Text.Json.Serialization;
using FinEdge.Application.Common.Models;
using FinEdge.Application.UserWallet.Queries;

namespace FinEdge.Application.UserWallet.Commands;

public class FundWalletCommand : IRequest<Result<GetWalletQueryResult>>
{
    [JsonIgnore]
    public int WalletId { get; set; }
    [JsonIgnore]
    public int UserId { get; set; }
    public decimal Amount { get; set; }
}

public class FundWalletCommandHandler(
    IWalletRepository walletRepository) : IRequestHandler<FundWalletCommand, Result<GetWalletQueryResult>>
{
    public async Task<Result<GetWalletQueryResult>> Handle(FundWalletCommand request, CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetByIdAsync(request.WalletId)
            ?? throw new NotFoundException(nameof(Wallet), request.WalletId);

        if (wallet.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("User does not own this wallet");
        }

        wallet.Balance += request.Amount;
        var response = await walletRepository.UpdateAsync(wallet);
        if (response == 0)
        {
            return Result<GetWalletQueryResult>.Failure(["failed_to_fund_wallet"]);
        }

        var newBalance = new GetWalletQueryResult
        {
            Id = wallet.Id,
            Balance = wallet.Balance
        };

        return Result<GetWalletQueryResult>.Success(newBalance);
    }
}
