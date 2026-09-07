namespace CooperativaElProgreso.IInterfaces;

/// <summary>
/// Registers deposits and withdrawals, enforcing the rules that decide whether a
/// movement is accepted (and recorded) or rejected (and never persisted).
/// </summary>
public interface IMovementService
{
    OperationResult<Movement> RegisterDeposit(string documentNumber, decimal amount);
    OperationResult<Movement> RegisterWithdrawal(string documentNumber, decimal amount);
    List<Movement> GetMovementsForMember(string documentNumber);
}
