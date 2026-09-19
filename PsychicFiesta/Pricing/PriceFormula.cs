using PsychicFiesta.Domain;

namespace PsychicFiesta.Pricing;

public sealed record RentalTariff
{
    public CarCategory CarCategory { get; }
    public RentalRates Rates { get; }
    public decimal DayFactor { get; }
    public decimal KmFactor { get; }

    public RentalTariff(
        CarCategory carCategory,
        RentalRates rates,
        decimal dayFactor,
        decimal kmFactor)
    {
        ArgumentNullException.ThrowIfNull(carCategory);
        ArgumentNullException.ThrowIfNull(rates);
        ArgumentOutOfRangeException.ThrowIfNegative(dayFactor);
        ArgumentOutOfRangeException.ThrowIfNegative(kmFactor);

        CarCategory = carCategory;
        Rates = rates;
        DayFactor = dayFactor;
        KmFactor = kmFactor;
    }

    public decimal CalculatePrice(int numberOfDays, int numberOfKm)
        => CalculateBreakdown(numberOfDays, numberOfKm).Total;

    public RentalPrice CalculateBreakdown(int numberOfDays, int numberOfKm)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(numberOfDays, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(numberOfKm);

        decimal dayCharge = RoundToCurrency(Rates.BaseDayRental * DayFactor * numberOfDays);
        decimal kmCharge = RoundToCurrency(Rates.BaseKmPrice * KmFactor * numberOfKm);

        return new RentalPrice(
            numberOfDays,
            numberOfKm,
            dayCharge,
            kmCharge,
            dayCharge + kmCharge);
    }

    private static decimal RoundToCurrency(decimal amount) =>
        Math.Round(amount, 2, MidpointRounding.AwayFromZero);
}