namespace PsychicFiesta.Domain;

public sealed record CarCategory
{
    public string Code { get; }

    public CarCategory(string code)
    {
        ArgumentNullException.ThrowIfNull(code);
        Code = code;
    }

    public static CarCategory SmallCar => new CarCategory("Small-car");
    public static CarCategory Combi => new CarCategory("Combi");
    public static CarCategory Truck => new CarCategory("Truck");

    public override string ToString() => Code;
}