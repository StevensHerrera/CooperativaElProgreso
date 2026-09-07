namespace CooperativaElProgreso.Models.Reports;

/// <summary>Answers "How did we do in a period?" for a given date range.</summary>
public record PeriodReport(
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalDeposited,
    decimal TotalWithdrawn,
    decimal Difference,
    int DepositCount,
    int WithdrawalCount);
