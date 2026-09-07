namespace CooperativaElProgreso.IInterfaces;

/// <summary>Persistence contract for movements. Movements are append-only: no update or delete.</summary>
public interface IMovementRepository
{
    Movement Add(Movement movement);
    List<Movement> GetAll();
    List<Movement> GetByMemberId(Guid memberId);
    bool HasAnyForMember(Guid memberId);
}
