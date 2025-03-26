using FinEdge.Domain.Enums;

namespace FinEdge.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public required int WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;
    public required TransactionType Type { get; set; }
    public required decimal Amount { get; set; }
    public required TransactionStatus Status { get; set; }
    public string FailureReason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
