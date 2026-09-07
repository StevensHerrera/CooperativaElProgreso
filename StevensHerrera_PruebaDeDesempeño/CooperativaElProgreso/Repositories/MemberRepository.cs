namespace CooperativaElProgreso.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly InMemoryStore _store;

    public MemberRepository(InMemoryStore store)
    {
        _store = store;
    }

    public Member Create(Member entity)
    {
        _store.Members.Add(entity);
        return entity;
    }

    public List<Member> GetAll() => _store.Members.ToList();

    public Member? GetById(Guid id) =>
        _store.Members.FirstOrDefault(m => m.Id == id);

    public bool Update(Member entity)
    {
        var existing = GetById(entity.Id);
        if (existing is null) return false;

        existing.FullName = entity.FullName;
        existing.PhoneNumber = entity.PhoneNumber;
        existing.Address = entity.Address;
        return true;
    }

    public bool Delete(Guid id)
    {
        var existing = GetById(id);
        if (existing is null) return false;
        return _store.Members.Remove(existing);
    }

    public Member? GetByDocumentNumber(string documentNumber) =>
        _store.Members.FirstOrDefault(m =>
            m.DocumentNumber.Equals(documentNumber, StringComparison.OrdinalIgnoreCase));

    public List<Member> SearchByName(string namePart) =>
        _store.Members
            .Where(m => m.FullName.Contains(namePart, StringComparison.OrdinalIgnoreCase))
            .ToList();

    public bool ExistsByDocumentNumber(string documentNumber) =>
        GetByDocumentNumber(documentNumber) is not null;
}
