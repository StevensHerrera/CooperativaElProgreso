namespace CooperativaElProgreso.IInterfaces;

/// <summary>Persistence contract for members. Implemented in-memory in this project.</summary>
public interface IMemberRepository : ICrudOperations<Member>
{
    /// <summary>Exact lookup by document number, used at the teller window.</summary>
    Member? GetByDocumentNumber(string documentNumber);

    /// <summary>Case-insensitive partial match on full name.</summary>
    List<Member> SearchByName(string namePart);

    bool ExistsByDocumentNumber(string documentNumber);
}
