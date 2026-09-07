namespace CooperativaElProgreso.Models.Reports;

/// <summary>Answers "How much money do we have?"</summary>
public record CooperativeSummaryReport(
    decimal TotalBalance,
    int MemberCount,
    decimal AverageBalance);
