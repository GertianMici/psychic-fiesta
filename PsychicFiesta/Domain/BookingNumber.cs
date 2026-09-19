namespace PsychicFiesta.Domain;

public sealed record BookingNumber
{
    public string Value { get; }

    public BookingNumber(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value.Trim().ToUpperInvariant();
    }

    public override string ToString() => Value;
}