namespace CooperativaElProgreso.IInterfaces;

/// <summary>Generic CRUD contract shared by repositories that expose a Guid-keyed entity.</summary>
public interface ICrudOperations<T> where T : class
{
    T Create(T entity);
    List<T> GetAll();
    T? GetById(Guid id);
    bool Update(T entity);
    bool Delete(Guid id);
}
