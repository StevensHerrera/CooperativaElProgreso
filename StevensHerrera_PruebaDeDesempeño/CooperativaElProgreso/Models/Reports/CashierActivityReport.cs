namespace CooperativaElProgreso.Models.Reports;

/// <summary>One row of the "Who is moving the cash register?" per-member activity report.</summary>
public record CashierActivityReport(
    string FullName,
    int MovementCount,
    decimal TotalDeposited,
    decimal TotalWithdrawn,
    decimal CurrentBalance);
