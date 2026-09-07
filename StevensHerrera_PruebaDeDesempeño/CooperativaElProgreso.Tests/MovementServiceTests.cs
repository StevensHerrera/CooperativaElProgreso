namespace CooperativaElProgreso.Tests;

public class MovementServiceTests
{
    private static (MemberService Members, MovementService Movements) BuildServices()
    {
        var store = new InMemoryStore();
        IMemberRepository memberRepository = new MemberRepository(store);
        IMovementRepository movementRepository = new MovementRepository(store);

        var memberService = new MemberService(
            memberRepository, movementRepository, new Fakes.FakeExchangeRateService());
        var movementService = new MovementService(memberRepository, movementRepository);

        memberService.RegisterMember("1001", "Maria Perez", null, null);
        return (memberService, movementService);
    }

    [Fact]
    public void Deposit_IncreasesBalanceByTheFullAmount()
    {
        var (members, movements) = BuildServices();

        movements.RegisterDeposit("1001", 500_000);

        var member = members.FindByDocumentNumber("1001")!;
        Assert.Equal(500_000, members.GetBalance(member.Id));
    }

    [Fact]
    public void Deposit_RejectsZeroOrNegativeAmounts()
    {
        var (_, movements) = BuildServices();

        var result = movements.RegisterDeposit("1001", 0);

        Assert.False(result.Success);
    }

    [Fact]
    public void Withdrawal_AboveThreshold_ChargesEightThousandFee()
    {
        var (members, movements) = BuildServices();
        movements.RegisterDeposit("1001", 2_000_000);

        movements.RegisterWithdrawal("1001", 1_500_000);

        var member = members.FindByDocumentNumber("1001")!;
        // 2,000,000 - 1,500,000 - 8,000 fee = 492,000
        Assert.Equal(492_000, members.GetBalance(member.Id));
    }

    [Fact]
    public void Withdrawal_AtOrBelowThreshold_ChargesNoFee()
    {
        var (members, movements) = BuildServices();
        movements.RegisterDeposit("1001", 2_000_000);

        movements.RegisterWithdrawal("1001", 1_000_000);

        var member = members.FindByDocumentNumber("1001")!;
        Assert.Equal(1_000_000, members.GetBalance(member.Id));
    }

    [Fact]
    public void Withdrawal_IsRejectedAndNotPersistedWhenItWouldGoNegative()
    {
        var (members, movements) = BuildServices();
        movements.RegisterDeposit("1001", 100_000);

        var result = movements.RegisterWithdrawal("1001", 150_000);

        var member = members.FindByDocumentNumber("1001")!;
        Assert.False(result.Success);
        Assert.Equal(100_000, members.GetBalance(member.Id));
        Assert.Empty(movements.GetMovementsForMember("1001").Where(m => m.Type == MovementType.Withdrawal));
    }

    [Fact]
    public void Withdrawal_IsRejectedWhenFeeWouldPushBalanceNegative()
    {
        // Balance exactly covers the withdrawal amount but not the added fee.
        var (members, movements) = BuildServices();
        movements.RegisterDeposit("1001", 1_500_000);

        var result = movements.RegisterWithdrawal("1001", 1_500_000);

        var member = members.FindByDocumentNumber("1001")!;
        Assert.False(result.Success);
        Assert.Equal(1_500_000, members.GetBalance(member.Id));
    }
}
