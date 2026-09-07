namespace CooperativaElProgreso.Data;

/// <summary>
/// Single in-memory data store shared by all repositories for the lifetime of the process.
/// </summary>
public class InMemoryStore
{
    public List<Member> Members { get; } = new();
    public List<Movement> Movements { get; } = new();
}
