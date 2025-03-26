namespace FinEdge.Domain.Entities;

public class Wallet
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public decimal Balance { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<Transaction> Transactions { get; set; } = [];
}
