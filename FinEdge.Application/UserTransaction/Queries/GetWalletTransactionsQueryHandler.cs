using FinEdge.Application.Common.Interfaces;
using FinEdge.Application.Common.Models;
using FinEdge.Application.UserTransaction.Commands;
using FinEdge.Domain.Entities;
using MediatR;
using NotFoundException = FinEdge.Application.Exceptions.NotFoundException;
using UnauthorizedAccessException = FinEdge.Application.Exceptions.UnauthorizedAccessException;

namespace FinEdge.Application.UserTransaction.Queries;

public record GetWalletTransactionsQuery(int WalletId, int UserId) : IRequest<Result<IEnumerable<TransactionResult>>>;

public class GetWalletTransactionsQueryHandler(
    IWalletRepository walletRepository,
    IUserRepository userRepository,
    ITransactionRepository transactionRepository) : IRequestHandler<GetWalletTransactionsQuery, Result<IEnumerable<TransactionResult>>>
{
    public async Task<Result<IEnumerable<TransactionResult>>> Handle(GetWalletTransactionsQuery request, CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetByIdAsync(request.WalletId, cancellationToken);
        if (wallet == null)
        {
            throw new NotFoundException(nameof(Wallet), request.WalletId);
        }

        if (wallet.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("User does not own this wallet");
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<IEnumerable<TransactionResult>>.Failure(["user_not_found"]);

        var transactions = await transactionRepository
            .GetByWalletIdAsync(request.WalletId, cancellationToken);

        var response = transactions.Select(t => new TransactionResult
        {
            Id = t.Id,
            Type = t.Type.ToString(),
            Amount = t.Amount,
            Status = t.Status.ToString(),
            FailureReason = string.IsNullOrEmpty(t.FailureReason)
            ? null
            : t.FailureReason,
            CreatedAt = t.CreatedAt
        }).ToList();

        return Result<IEnumerable<TransactionResult>>.Success(response);
    }
}
