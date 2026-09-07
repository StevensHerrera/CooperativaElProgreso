namespace CooperativaElProgreso.Models;

/// <summary>
/// Outcome of an operation that can be rejected by a business rule (e.g. a movement).
/// On rejection, Payload is null, nothing was persisted, and Message explains why to the teller.
/// </summary>
public class OperationResult<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Payload { get; init; }

    public static OperationResult<T> Ok(T payload, string? message = null) =>
        new() { Success = true, Payload = payload, Message = message };

    public static OperationResult<T> Fail(string message) =>
        new() { Success = false, Message = message };
}
