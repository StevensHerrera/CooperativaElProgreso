namespace CooperativaElProgreso.Models;

/// <summary>A member's balance converted to USD, together with the rate that produced it.</summary>
public record UsdBalanceResult(decimal BalanceInCop, decimal BalanceInUsd, ExchangeRate RateUsed);
