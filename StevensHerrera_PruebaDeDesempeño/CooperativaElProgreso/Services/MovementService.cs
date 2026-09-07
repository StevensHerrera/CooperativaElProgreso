namespace CooperativaElProgreso.Services;

public class MovementService : IMovementService
{
    private const decimal WithdrawalFeeThreshold = 1_000_000m;
    private const decimal WithdrawalFee = 8_000m;

    private readonly IMemberRepository _memberRepository;
    private readonly IMovementRepository _movementRepository;

    public MovementService(IMemberRepository memberRepository, IMovementRepository movementRepository)
    {
        _memberRepository = memberRepository;
        _movementRepository = movementRepository;
    }

    public OperationResult<Movement> RegisterDeposit(string documentNumber, decimal amount)
    {
        var member = _memberRepository.GetByDocumentNumber(documentNumber);
        if (member is null)
        {
            return OperationResult<Movement>.Fail($"No se encontró un asociado con el número de documento '{documentNumber}'.");
        }

        if (amount <= 0)
        {
            return OperationResult<Movement>.Fail("El monto de la consignación debe ser mayor que cero.");
        }

        var movement = new Movement
        {
            MemberId = member.Id,
            Type = MovementType.Deposit,
            Amount = amount,
            Fee = 0
        };

        _movementRepository.Add(movement);
        return OperationResult<Movement>.Ok(movement, "Consignación registrada.");
    }

    public OperationResult<Movement> RegisterWithdrawal(string documentNumber, decimal amount)
    {
        var member = _memberRepository.GetByDocumentNumber(documentNumber);
        if (member is null)
        {
            return OperationResult<Movement>.Fail($"No se encontró un asociado con el número de documento '{documentNumber}'.");
        }

        if (amount <= 0)
        {
            return OperationResult<Movement>.Fail("El monto del retiro debe ser mayor que cero.");
        }

        var fee = amount > WithdrawalFeeThreshold ? WithdrawalFee : 0m;
        var currentBalance = _movementRepository.GetByMemberId(member.Id).Sum(m => m.SignedTotal);
        var totalToDeduct = amount + fee;

        if (totalToDeduct > currentBalance)
        {
            return OperationResult<Movement>.Fail(
                $"Saldo insuficiente. El saldo actual es COP $ {currentBalance:N0}, pero se solicitaron COP $ {totalToDeduct:N0} " +
                $"(retiro{(fee > 0 ? " + comisión de manejo de efectivo de COP $8.000" : string.Empty)}). " +
                "La cuenta no puede quedar en negativo, por lo tanto el retiro no fue ejecutado.");
        }

        var movement = new Movement
        {
            MemberId = member.Id,
            Type = MovementType.Withdrawal,
            Amount = amount,
            Fee = fee
        };

        _movementRepository.Add(movement);

        var message = fee > 0
            ? "Retiro registrado con una comisión de manejo de efectivo de COP $8.000."
            : "Retiro registrado.";
        return OperationResult<Movement>.Ok(movement, message);
    }

    public List<Movement> GetMovementsForMember(string documentNumber)
    {
        var member = _memberRepository.GetByDocumentNumber(documentNumber);
        return member is null ? new List<Movement>() : _movementRepository.GetByMemberId(member.Id);
    }
}
