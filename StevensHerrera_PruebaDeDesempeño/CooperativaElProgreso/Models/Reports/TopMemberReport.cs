namespace CooperativaElProgreso.Models.Reports;

/// <summary>One row of the "Who are my best members?" report.</summary>
public record TopMemberReport(
    string DocumentNumber,
    string FullName,
    decimal Balance);
