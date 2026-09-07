namespace CooperativaElProgreso.Tests;

public class ReportServiceTests
{
    private static (MemberService Members, MovementService Movements, ReportService Reports) BuildServices()
    {
        var store = new InMemoryStore();
        IMemberRepository memberRepository = new MemberRepository(store);
        IMovementRepository movementRepository = new MovementRepository(store);

        var memberService = new MemberService(
            memberRepository, movementRepository, new Fakes.FakeExchangeRateService());
        var movementService = new MovementService(memberRepository, movementRepository);
        var reportService = new ReportService(memberRepository, movementRepository);

        return (memberService, movementService, reportService);
    }

    [Fact]
    public void CooperativeSummary_AggregatesBalancesAcrossAllMembers()
    {
        var (members, movements, reports) = BuildServices();
        members.RegisterMember("1001", "Maria Perez", null, null);
        members.RegisterMember("1002", "Carlos Ruiz", null, null);
        movements.RegisterDeposit("1001", 300_000);
        movements.RegisterDeposit("1002", 700_000);

        var summary = reports.GetCooperativeSummary();

        Assert.Equal(1_000_000, summary.TotalBalance);
        Assert.Equal(2, summary.MemberCount);
        Assert.Equal(500_000, summary.AverageBalance);
    }

    [Fact]
    public void TopMembers_ReturnsHighestBalancesFirst()
    {
        var (members, movements, reports) = BuildServices();
        members.RegisterMember("1001", "Maria Perez", null, null);
        members.RegisterMember("1002", "Carlos Ruiz", null, null);
        movements.RegisterDeposit("1001", 100_000);
        movements.RegisterDeposit("1002", 900_000);

        var top = reports.GetTopMembers();

        Assert.Equal("1002", top[0].DocumentNumber);
    }

    [Fact]
    public void DormantMembers_OnlyIncludesMembersWithNoMovements()
    {
        var (members, movements, reports) = BuildServices();
        members.RegisterMember("1001", "Maria Perez", null, null);
        members.RegisterMember("1002", "Carlos Ruiz", null, null);
        movements.RegisterDeposit("1001", 100_000);

        var dormant = reports.GetDormantMembers();

        Assert.Single(dormant);
        Assert.Equal("1002", dormant[0].DocumentNumber);
    }

    [Fact]
    public void CashierActivity_OrdersByMovementCountDescending()
    {
        var (members, movements, reports) = BuildServices();
        members.RegisterMember("1001", "Maria Perez", null, null);
        members.RegisterMember("1002", "Carlos Ruiz", null, null);
        movements.RegisterDeposit("1001", 100_000);
        movements.RegisterDeposit("1002", 100_000);
        movements.RegisterDeposit("1002", 50_000);

        var activity = reports.GetCashierActivity();

        Assert.Equal("Carlos Ruiz", activity[0].FullName);
        Assert.Equal(2, activity[0].MovementCount);
    }
}
