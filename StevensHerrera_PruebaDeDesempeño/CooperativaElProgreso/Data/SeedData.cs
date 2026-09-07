namespace CooperativaElProgreso.Data;

/// <summary>
/// Populates an InMemoryStore with sample members and movements, for local testing only.
/// </summary>
public static class SeedData
{
    public static void Apply(InMemoryStore store)
    {
        var maria = new Member
        {
            DocumentNumber = "1001",
            FullName = "Maria Restrepo",
            PhoneNumber = "3001234567",
            Address = "Calle 10 # 5-20",
            RegisteredAt = DateTime.Now.AddMonths(-6)
        };

        var carlos = new Member
        {
            DocumentNumber = "1002",
            FullName = "Carlos Vanegas",
            PhoneNumber = "3007654321",
            Address = "Carrera 44 # 12-30",
            RegisteredAt = DateTime.Now.AddMonths(-3)
        };

        // Dormant on purpose: registered, but never touched again — for the
        // "who is dormant?" report.
        var ana = new Member
        {
            DocumentNumber = "1003",
            FullName = "Ana Diaz",
            PhoneNumber = "3009998888",
            Address = "Avenida Circunvalar # 1-01",
            RegisteredAt = DateTime.Now.AddMonths(-2)
        };

        store.Members.AddRange(new[] { maria, carlos, ana });

        store.Movements.AddRange(new[]
        {
            new Movement { MemberId = maria.Id, Type = MovementType.Deposit, Amount = 800_000, Date = DateTime.Now.AddMonths(-6) },
            new Movement { MemberId = maria.Id, Type = MovementType.Withdrawal, Amount = 100_000, Date = DateTime.Now.AddMonths(-4) },
            new Movement { MemberId = maria.Id, Type = MovementType.Deposit, Amount = 200_000, Date = DateTime.Now.AddDays(-10) },

            new Movement { MemberId = carlos.Id, Type = MovementType.Deposit, Amount = 2_000_000, Date = DateTime.Now.AddMonths(-3) },
            new Movement { MemberId = carlos.Id, Type = MovementType.Withdrawal, Amount = 1_500_000, Fee = 8_000, Date = DateTime.Now.AddDays(-5) },
        });
    }
}
