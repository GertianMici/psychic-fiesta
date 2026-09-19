namespace PsychicFiesta.Domain;

public sealed record PersonalIdentityNumber
{
    private string Value { get; }

    public PersonalIdentityNumber(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value.Trim();
    }

    private string MaskedValue => Value.Length > 4
        ? string.Concat(Value.AsSpan(0, Value.Length - 4), "****")
        : "****";

    public string GetRawValue() => Value;

    public override string ToString() => MaskedValue;
}