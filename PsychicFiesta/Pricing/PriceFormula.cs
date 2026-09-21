using PsychicFiesta.Domain;

namespace PsychicFiesta.Pricing;

public sealed record PriceFormula
{
    public CarCategory CarCategory { get; }
    public BaseRates BaseRates { get; }
    public decimal DayFactor { get; }
    public decimal KmFactor { get; }

    public PriceFormula(
        CarCategory carCategory,
        BaseRates baseRates,
        decimal dayFactor,
        decimal kmFactor)
    {
        ArgumentNullException.ThrowIfNull(carCategory);
        ArgumentNullException.ThrowIfNull(baseRates);
        ArgumentOutOfRangeException.ThrowIfNegative(dayFactor);
        ArgumentOutOfRangeException.ThrowIfNegative(kmFactor);

        CarCategory = carCategory;
        BaseRates = baseRates;
        DayFactor = dayFactor;
        KmFactor = kmFactor;
    }

    public decimal CalculatePrice(int numberOfDays, int numberOfKm)
        => CalculateBreakdown(numberOfDays, numberOfKm).Total;

    public RentalPrice CalculateBreakdown(int numberOfDays, int numberOfKm)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(numberOfDays, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(numberOfKm);

        decimal dayCharge = RoundToCurrency(BaseRates.BaseDayRental * DayFactor * numberOfDays);
        decimal kmCharge = RoundToCurrency(BaseRates.BaseKmPrice * KmFactor * numberOfKm);

        return new RentalPrice(
            numberOfDays,
            numberOfKm,
            dayCharge,
            kmCharge);
    }

    private static decimal RoundToCurrency(decimal amount) =>
        Math.Round(amount, 2, MidpointRounding.AwayFromZero);
}