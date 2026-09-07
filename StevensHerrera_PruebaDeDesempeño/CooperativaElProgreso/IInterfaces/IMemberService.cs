namespace CooperativaElProgreso.IInterfaces;

/// <summary>
/// Member management and balance queries. Balance is always computed from movements,
/// never stored or edited directly, per the cooperative's business rules.
/// </summary>
public interface IMemberService
{
    OperationResult<Member> RegisterMember(string documentNumber, string fullName, string? phoneNumber, string? address);
    List<Member> ListMembers();
    Member? FindByDocumentNumber(string documentNumber);
    List<Member> FindByName(string namePart);
    OperationResult<Member> UpdateMember(string documentNumber, string? fullName, string? phoneNumber, string? address);
    OperationResult<bool> DeleteMember(string documentNumber);
    decimal GetBalance(Guid memberId);

    /// <summary>Converts a member's balance to USD using the official TRM. Never throws on API failure.</summary>
    Task<OperationResult<UsdBalanceResult>> GetBalanceInUsdAsync(string documentNumber);
}
