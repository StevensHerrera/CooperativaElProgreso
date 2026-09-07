namespace CooperativaElProgreso.Services;

public class ReportService : IReportService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMovementRepository _movementRepository;

    public ReportService(IMemberRepository memberRepository, IMovementRepository movementRepository)
    {
        _memberRepository = memberRepository;
        _movementRepository = movementRepository;
    }

    private decimal BalanceOf(Guid memberId) =>
        _movementRepository.GetByMemberId(memberId).Sum(m => m.SignedTotal);

    /// <summary>"How much money do we have?"</summary>
    public CooperativeSummaryReport GetCooperativeSummary()
    {
        var members = _memberRepository.GetAll();
        var totalBalance = members.Sum(m => BalanceOf(m.Id));
        var averageBalance = members.Count == 0 ? 0 : Math.Round(totalBalance / members.Count, 2);

        return new CooperativeSummaryReport(totalBalance, members.Count, averageBalance);
    }

    /// <summary>"Who are my best members?" — top N by balance, descending.</summary>
    public List<TopMemberReport> GetTopMembers(int count = 5) =>
        _memberRepository.GetAll()
            .Select(m => new TopMemberReport(m.DocumentNumber, m.FullName, BalanceOf(m.Id)))
            .OrderByDescending(r => r.Balance)
            .Take(count)
            .ToList();

    /// <summary>"Who is dormant?" — members with no movements at all since they joined.</summary>
    public List<DormantMemberReport> GetDormantMembers() =>
        _memberRepository.GetAll()
            .Where(m => !_movementRepository.HasAnyForMember(m.Id))
            .Select(m => new DormantMemberReport(m.DocumentNumber, m.FullName, m.RegisteredAt))
            .ToList();

    /// <summary>"How did we do in a period?" — deposits vs. withdrawals within [startDate, endDate].</summary>
    public PeriodReport GetPeriodReport(DateTime startDate, DateTime endDate)
    {
        var inclusiveEnd = endDate.Date.AddDays(1).AddTicks(-1);
        var movementsInRange = _movementRepository.GetAll()
            .Where(m => m.Date >= startDate.Date && m.Date <= inclusiveEnd)
            .ToList();

        var deposits = movementsInRange.Where(m => m.Type == MovementType.Deposit).ToList();
        var withdrawals = movementsInRange.Where(m => m.Type == MovementType.Withdrawal).ToList();

        var totalDeposited = deposits.Sum(m => m.Amount);
        var totalWithdrawn = withdrawals.Sum(m => m.Amount + m.Fee);

        return new PeriodReport(
            startDate.Date,
            endDate.Date,
            totalDeposited,
            totalWithdrawn,
            totalDeposited - totalWithdrawn,
            deposits.Count,
            withdrawals.Count);
    }

    /// <summary>"What were the biggest movements?" — top N by amount, cooperative-wide.</summary>
    public List<TopMovementReport> GetTopMovements(int count = 10)
    {
        var members = _memberRepository.GetAll().ToDictionary(m => m.Id, m => m.FullName);

        return _movementRepository.GetAll()
            .OrderByDescending(m => m.Amount)
            .Take(count)
            .Select(m => new TopMovementReport(
                m.Date,
                m.Type,
                m.Amount,
                members.TryGetValue(m.MemberId, out var name) ? name : "Unknown member"))
            .ToList();
    }

    /// <summary>"Who is moving the cash register?" — per-member activity, busiest first.</summary>
    public List<CashierActivityReport> GetCashierActivity() =>
        _memberRepository.GetAll()
            .Select(m =>
            {
                var movements = _movementRepository.GetByMemberId(m.Id);
                return new CashierActivityReport(
                    m.FullName,
                    movements.Count,
                    movements.Where(x => x.Type == MovementType.Deposit).Sum(x => x.Amount),
                    movements.Where(x => x.Type == MovementType.Withdrawal).Sum(x => x.Amount + x.Fee),
                    movements.Sum(x => x.SignedTotal));
            })
            .OrderByDescending(r => r.MovementCount)
            .ToList();
}
