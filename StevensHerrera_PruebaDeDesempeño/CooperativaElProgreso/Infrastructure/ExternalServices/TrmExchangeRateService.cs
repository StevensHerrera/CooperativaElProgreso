namespace CooperativaElProgreso.Infrastructure.ExternalServices;

/// <summary>
/// Fetches the official TRM from Colombia's open-data portal (Superintendencia Financiera).
/// This is infrastructure, not business logic: it only knows how to talk to the external API
/// and translate the response into a domain-level ExchangeRate.
/// </summary>
public class TrmExchangeRateService : IExchangeRateService, IDisposable
{
    private const string RequestUri =
        "https://www.datos.gov.co/resource/32sa-8pi3.json?$order=vigenciadesde%20DESC&$limit=1";

    private readonly HttpClient _httpClient;

    public TrmExchangeRateService(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
    }

    public async Task<OperationResult<ExchangeRate>> GetOfficialRateAsync()
    {
        try
        {
            using var response = await _httpClient.GetAsync(RequestUri);
            if (!response.IsSuccessStatusCode)
            {
                return OperationResult<ExchangeRate>.Fail(
                    $"El servicio de TRM respondió con el estado {(int)response.StatusCode}.");
            }

            var body = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(body);

            if (document.RootElement.ValueKind != JsonValueKind.Array || document.RootElement.GetArrayLength() == 0)
            {
                return OperationResult<ExchangeRate>.Fail("El servicio de TRM no devolvió datos.");
            }

            var record = document.RootElement[0];
            var value = decimal.Parse(record.GetProperty("valor").GetString()!, CultureInfo.InvariantCulture);
            var validFrom = DateTime.Parse(record.GetProperty("vigenciadesde").GetString()!, CultureInfo.InvariantCulture);
            var validTo = DateTime.Parse(record.GetProperty("vigenciahasta").GetString()!, CultureInfo.InvariantCulture);

            return OperationResult<ExchangeRate>.Ok(new ExchangeRate(value, validFrom, validTo));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or FormatException)
        {
            // The application must never crash because the external rate is unavailable;
            // the caller decides how to inform the teller and keeps running normally.
            return OperationResult<ExchangeRate>.Fail("No se pudo contactar el servicio de TRM oficial. Inténtelo más tarde.");
        }
    }

    public void Dispose() => _httpClient.Dispose();
}
