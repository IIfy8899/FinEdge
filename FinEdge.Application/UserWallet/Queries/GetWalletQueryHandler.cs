using FinEdge.Application.Common.Interfaces;
using FinEdge.Application.Common.Models;
using FinEdge.Domain.Entities;
using MediatR;
using NotFoundException = FinEdge.Application.Exceptions.NotFoundException;
using UnauthorizedAccessException = FinEdge.Application.Exceptions.UnauthorizedAccessException;

namespace FinEdge.Application.UserWallet.Queries;

public record GetWalletQuery(int WalletId, int UserId) : IRequest<Result<GetWalletQueryResult>>;

public class GetWalletQueryResult
{
    public int Id { get; set; }
    public decimal Balance { get; set; }
}

public class GetWalletQueryHandler(
    IWalletRepository walletRepository) : IRequestHandler<GetWalletQuery, Result<GetWalletQueryResult>>
{
    public async Task<Result<GetWalletQueryResult>> Handle(GetWalletQuery request, CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetByIdAsync(request.WalletId, cancellationToken)
            ?? throw new NotFoundException(nameof(Wallet), request.WalletId);

        if (wallet.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("User does not own this wallet");
        }

        return Result<GetWalletQueryResult>.Success(new()
        {
            Id = wallet.Id,
            Balance = wallet.Balance
        });
    }
}
