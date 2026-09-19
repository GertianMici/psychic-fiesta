namespace PsychicFiesta.Pricing;

public sealed record RentalRates
{
    public decimal BaseDayRental { get; }
    public decimal BaseKmPrice { get; }

    public RentalRates(decimal baseDayRental, decimal baseKmPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(baseDayRental);
        ArgumentOutOfRangeException.ThrowIfNegative(baseKmPrice);

        BaseDayRental = baseDayRental;
        BaseKmPrice = baseKmPrice;
    }
}