namespace CooperativaElProgreso.Tests.Fakes;

/// <summary>
/// Deterministic stand-in for TrmExchangeRateService, so unit tests never depend on
/// network access. Configure Result before calling the method under test.
/// </summary>
public class FakeExchangeRateService : IExchangeRateService
{
    public OperationResult<ExchangeRate> Result { get; set; } =
        OperationResult<ExchangeRate>.Ok(new ExchangeRate(4000m, DateTime.Today, DateTime.Today));

    public Task<OperationResult<ExchangeRate>> GetOfficialRateAsync() => Task.FromResult(Result);
}
