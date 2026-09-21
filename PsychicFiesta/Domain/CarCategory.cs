namespace PsychicFiesta.Domain;

public sealed record CarCategory
{
    public string Code { get; }

    public CarCategory(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        Code = code.Trim().ToLowerInvariant();
    }

    public override string ToString() => Code;
}