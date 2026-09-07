namespace CooperativaElProgreso.Models;

/// <summary>
/// A cooperative member (associate) who holds exactly one savings account.
/// The account balance is never stored here: it is always derived from the
/// member's recorded movements, so it cannot be edited by hand.
/// </summary>
public class Member
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string DocumentNumber { get; set; }
    public required string FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime RegisteredAt { get; init; } = DateTime.Now;
}
