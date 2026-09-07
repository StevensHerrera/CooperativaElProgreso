namespace CooperativaElProgreso.Models.Reports;

/// <summary>One row of the "Who is dormant?" report: members with zero movements ever.</summary>
public record DormantMemberReport(
    string DocumentNumber,
    string FullName,
    DateTime RegisteredAt);
