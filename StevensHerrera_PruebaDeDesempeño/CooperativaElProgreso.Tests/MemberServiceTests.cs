using CooperativaElProgreso.Tests.Fakes;

namespace CooperativaElProgreso.Tests;

public class MemberServiceTests
{
    private static (MemberService Members, MovementService Movements) BuildServices()
    {
        var store = new InMemoryStore();
        IMemberRepository memberRepository = new MemberRepository(store);
        IMovementRepository movementRepository = new MovementRepository(store);

        var memberService = new MemberService(memberRepository, movementRepository, new FakeExchangeRateService());
        var movementService = new MovementService(memberRepository, movementRepository);

        return (memberService, movementService);
    }

    [Fact]
    public void RegisterMember_StartsWithZeroBalance()
    {
        var (members, _) = BuildServices();

        var result = members.RegisterMember("1001", "Maria Perez", null, null);

        Assert.True(result.Success);
        Assert.Equal(0, members.GetBalance(result.Payload!.Id));
    }

    [Fact]
    public void RegisterMember_RejectsDuplicateDocumentNumber()
    {
        var (members, _) = BuildServices();
        members.RegisterMember("1001", "Maria Perez", null, null);

        var result = members.RegisterMember("1001", "Another Person", null, null);

        Assert.False(result.Success);
    }

    [Fact]
    public void FindByName_IsCaseInsensitiveAndMatchesPartially()
    {
        var (members, _) = BuildServices();
        members.RegisterMember("1001", "Maria Perez", null, null);

        var matches = members.FindByName("maria");

        Assert.Single(matches);
    }

    [Fact]
    public void DeleteMember_IsRejectedWhenMemberHasMovements()
    {
        var (members, movements) = BuildServices();
        var member = members.RegisterMember("1001", "Maria Perez", null, null).Payload!;
        movements.RegisterDeposit("1001", 100_000);

        var result = members.DeleteMember("1001");

        Assert.False(result.Success);
    }

    [Fact]
    public void DeleteMember_SucceedsWhenNoMovementsExist()
    {
        var (members, _) = BuildServices();
        members.RegisterMember("1001", "Maria Perez", null, null);

        var result = members.DeleteMember("1001");

        Assert.True(result.Success);
    }
}
