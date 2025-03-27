using FinEdge.Application.Common.Interfaces;
using FinEdge.Application.Common.Models;
using FinEdge.Domain.Entities;
using FinEdge.Domain.Enums;
using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;
using NotFoundException = FinEdge.Application.Exceptions.NotFoundException;
using UnauthorizedAccessException = FinEdge.Application.Exceptions.UnauthorizedAccessException;

namespace FinEdge.Application.UserTransaction.Commands;

public class CreateTransactionCommand : IRequest<Result<TransactionResult>>
{
    [JsonIgnore]
    public int WalletId { get; set; }
    [JsonIgnore]
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("transaction_type_required")
            .Must(BeValidTransactionType)
            .WithMessage("invalid_transaction_type");
    }

    private bool BeValidTransactionType(string type)
    {
        var types = Enum.GetNames<TransactionType>().Where(t => t.ToLower() == type.ToLower()).ToArray();

        return types.Length > 0;
    }
}

public class TransactionResult
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTransactionCommandHandler(
    IWalletRepository walletRepository,
    ITransactionRepository transactionRepository)
    : IRequestHandler<CreateTransactionCommand, Result<TransactionResult>>
{
    public async Task<Result<TransactionResult>> Handle(
        CreateTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetByIdAsync(request.WalletId, cancellationToken)
            ?? throw new NotFoundException(nameof(Wallet), request.WalletId);

        if (wallet.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("User does not own this wallet");
        }

        bool isSuccessful = false;
        string failureReason = "";
        Transaction transactionEntity;

        if (request.Type == TransactionType.Power.ToString())
        {
            failureReason = "Power transactions always fail";
            isSuccessful = false;
        }
        else if (wallet.Balance >= request.Amount)
        {
            await using var dbTransaction = await walletRepository.BeginTransaction(cancellationToken);

            try
            {
                wallet.Balance -= request.Amount;
                var updateResult = await walletRepository.UpdateAsync(wallet, cancellationToken);

                if (updateResult == 0)
                {
                    failureReason = "Failed to update wallet balance";
                    isSuccessful = false;
                }
                else
                {
                    isSuccessful = true;
                    await dbTransaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await dbTransaction.RollbackAsync(cancellationToken);
                failureReason = "Error updating wallet balance";
                isSuccessful = false;
            }
        }
        else
        {
            failureReason = "Insufficient balance";
            isSuccessful = false;
        }

        transactionEntity = new Transaction
        {
            WalletId = request.WalletId,
            Type = Enum.Parse<TransactionType>(request.Type, true),
            Amount = request.Amount,
            Status = isSuccessful ? TransactionStatus.Success : TransactionStatus.Failed,
            FailureReason = failureReason
        };

        await transactionRepository.AddAsync(transactionEntity, cancellationToken);

        var response = new TransactionResult
        {
            Id = transactionEntity.Id,
            Type = transactionEntity.Type.ToString(),
            Amount = transactionEntity.Amount,
            Status = transactionEntity.Status.ToString(),
            FailureReason = string.IsNullOrEmpty(transactionEntity.FailureReason)
                ? null
                : transactionEntity.FailureReason,
            CreatedAt = transactionEntity.CreatedAt
        };

        return Result<TransactionResult>.Success(response);
    }
}
