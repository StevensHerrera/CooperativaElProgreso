namespace CooperativaElProgreso.Models.Reports;

/// <summary>One row of the "What were the biggest movements?" report (top 10 cooperative-wide).</summary>
public record TopMovementReport(
    DateTime Date,
    MovementType Type,
    decimal Amount,
    string MemberFullName);
