namespace PsychicFiesta.Pricing;

public sealed record BaseRates
{
    public decimal BaseDayRental { get; }
    public decimal BaseKmPrice { get; }

    public BaseRates(decimal baseDayRental, decimal baseKmPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(baseDayRental);
        ArgumentOutOfRangeException.ThrowIfNegative(baseKmPrice);

        BaseDayRental = baseDayRental;
        BaseKmPrice = baseKmPrice;
    }
}