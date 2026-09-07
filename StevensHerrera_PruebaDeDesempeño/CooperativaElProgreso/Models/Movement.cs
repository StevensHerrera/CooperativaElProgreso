namespace CooperativaElProgreso.Models;

/// <summary>
/// A single accepted transaction (deposit or withdrawal) against a member's account.
/// Rejected movements are never turned into a Movement instance: the balance rule
/// (see MovementService) validates before anything is created here.
/// </summary>
public class Movement
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid MemberId { get; init; }
    public required MovementType Type { get; init; }

    /// <summary>The amount the member handed over or received, before any fee.</summary>
    public required decimal Amount { get; init; }

    /// <summary>Cash-handling fee charged on withdrawals over the threshold. Zero otherwise.</summary>
    public decimal Fee { get; init; }

    public DateTime Date { get; init; } = DateTime.Now;

    /// <summary>Total effect on the balance: +Amount for deposits, -(Amount + Fee) for withdrawals.</summary>
    public decimal SignedTotal => Type == MovementType.Deposit ? Amount : -(Amount + Fee);
}
