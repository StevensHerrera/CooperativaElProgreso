namespace CooperativaElProgreso.IInterfaces;

/// <summary>Produces the six management reports requested by the cooperative's manager.</summary>
public interface IReportService
{
    CooperativeSummaryReport GetCooperativeSummary();
    List<TopMemberReport> GetTopMembers(int count = 5);
    List<DormantMemberReport> GetDormantMembers();
    PeriodReport GetPeriodReport(DateTime startDate, DateTime endDate);
    List<TopMovementReport> GetTopMovements(int count = 10);
    List<CashierActivityReport> GetCashierActivity();
}
