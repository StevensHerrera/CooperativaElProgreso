namespace CooperativaElProgreso.IInterfaces;

/// <summary>Asynchronous client for the official peso-to-dollar exchange rate (TRM).</summary>
public interface IExchangeRateService
{
    /// <summary>
    /// Fetches the current official rate. Never throws: on any failure (network, parsing,
    /// unavailable service) it returns a failed OperationResult so the caller can keep operating.
    /// </summary>
    Task<OperationResult<ExchangeRate>> GetOfficialRateAsync();
}
