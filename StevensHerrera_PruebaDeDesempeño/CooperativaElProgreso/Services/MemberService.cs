namespace CooperativaElProgreso.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMovementRepository _movementRepository;
    private readonly IExchangeRateService _exchangeRateService;

    public MemberService(
        IMemberRepository memberRepository,
        IMovementRepository movementRepository,
        IExchangeRateService exchangeRateService)
    {
        _memberRepository = memberRepository;
        _movementRepository = movementRepository;
        _exchangeRateService = exchangeRateService;
    }

    public OperationResult<Member> RegisterMember(string documentNumber, string fullName, string? phoneNumber, string? address)
    {
        if (string.IsNullOrWhiteSpace(documentNumber) || string.IsNullOrWhiteSpace(fullName))
        {
            return OperationResult<Member>.Fail("El número de documento y el nombre completo son obligatorios.");
        }

        if (_memberRepository.ExistsByDocumentNumber(documentNumber))
        {
            return OperationResult<Member>.Fail($"Ya existe un asociado con el número de documento '{documentNumber}'.");
        }

        var member = new Member
        {
            DocumentNumber = documentNumber.Trim(),
            FullName = fullName.Trim(),
            PhoneNumber = phoneNumber?.Trim(),
            Address = address?.Trim()
        };

        _memberRepository.Create(member);
        return OperationResult<Member>.Ok(member, "Asociado registrado con un saldo inicial de 0.");
    }

    public List<Member> ListMembers() => _memberRepository.GetAll();

    public Member? FindByDocumentNumber(string documentNumber) =>
        _memberRepository.GetByDocumentNumber(documentNumber);

    public List<Member> FindByName(string namePart) =>
        _memberRepository.SearchByName(namePart);

    public OperationResult<Member> UpdateMember(string documentNumber, string? fullName, string? phoneNumber, string? address)
    {
        var member = _memberRepository.GetByDocumentNumber(documentNumber);
        if (member is null)
        {
            return OperationResult<Member>.Fail($"No se encontró un asociado con el número de documento '{documentNumber}'.");
        }

        var updated = new Member
        {
            Id = member.Id,
            DocumentNumber = member.DocumentNumber,
            FullName = string.IsNullOrWhiteSpace(fullName) ? member.FullName : fullName.Trim(),
            PhoneNumber = phoneNumber is null ? member.PhoneNumber : phoneNumber.Trim(),
            Address = address is null ? member.Address : address.Trim()
        };

        _memberRepository.Update(updated);
        return OperationResult<Member>.Ok(updated);
    }

    public OperationResult<bool> DeleteMember(string documentNumber)
    {
        var member = _memberRepository.GetByDocumentNumber(documentNumber);
        if (member is null)
        {
            return OperationResult<bool>.Fail($"No se encontró un asociado con el número de documento '{documentNumber}'.");
        }

        if (_movementRepository.HasAnyForMember(member.Id))
        {
            return OperationResult<bool>.Fail("Este asociado no puede eliminarse porque tiene movimientos registrados.");
        }

        if (GetBalance(member.Id) != 0)
        {
            return OperationResult<bool>.Fail("Este asociado no puede eliminarse porque tiene un saldo diferente de cero.");
        }

        _memberRepository.Delete(member.Id);
        return OperationResult<bool>.Ok(true, "Asociado eliminado.");
    }

    public decimal GetBalance(Guid memberId) =>
        _movementRepository.GetByMemberId(memberId).Sum(m => m.SignedTotal);

    public async Task<OperationResult<UsdBalanceResult>> GetBalanceInUsdAsync(string documentNumber)
    {
        var member = _memberRepository.GetByDocumentNumber(documentNumber);
        if (member is null)
        {
            return OperationResult<UsdBalanceResult>.Fail($"No se encontró un asociado con el número de documento '{documentNumber}'.");
        }

        var rateResult = await _exchangeRateService.GetOfficialRateAsync();
        if (!rateResult.Success || rateResult.Payload is null)
        {
            return OperationResult<UsdBalanceResult>.Fail(
                rateResult.Message ?? "No se pudo obtener la TRM oficial.");
        }

        var balanceInCop = GetBalance(member.Id);
        var rate = rateResult.Payload;
        var balanceInUsd = rate.Value == 0 ? 0 : Math.Round(balanceInCop / rate.Value, 2);

        return OperationResult<UsdBalanceResult>.Ok(new UsdBalanceResult(balanceInCop, balanceInUsd, rate));
    }
}
