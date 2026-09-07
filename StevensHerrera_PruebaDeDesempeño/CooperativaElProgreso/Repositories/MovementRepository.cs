namespace CooperativaElProgreso.Repositories;

public class MovementRepository : IMovementRepository
{
    private readonly InMemoryStore _store;

    public MovementRepository(InMemoryStore store)
    {
        _store = store;
    }

    public Movement Add(Movement movement)
    {
        _store.Movements.Add(movement);
        return movement;
    }

    public List<Movement> GetAll() => _store.Movements.ToList();

    public List<Movement> GetByMemberId(Guid memberId) =>
        _store.Movements.Where(m => m.MemberId == memberId).ToList();

    public bool HasAnyForMember(Guid memberId) =>
        _store.Movements.Any(m => m.MemberId == memberId);
}
