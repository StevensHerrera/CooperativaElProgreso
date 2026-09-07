namespace CooperativaElProgreso.Models;

/// <summary>
/// The official TRM (Tasa Representativa del Mercado) for a validity period.
/// A single rate can be in effect for several days (e.g. Friday's rate holds through Sunday),
/// so the validity window is carried alongside the value for display purposes.
/// </summary>
public record ExchangeRate(decimal Value, DateTime ValidFrom, DateTime ValidTo);
