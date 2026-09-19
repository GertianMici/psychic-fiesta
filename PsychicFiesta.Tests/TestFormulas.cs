using PsychicFiesta.Application;
using PsychicFiesta.Domain;
using PsychicFiesta.Infrastructure;
using PsychicFiesta.Pricing;

namespace PsychicFiesta.Tests;

/// <summary>
/// Fixed rates for tests. The values are chosen so that the three categories produce three
/// DIFFERENT totals for the same days and distance, which means a formula wired to the wrong
/// category cannot pass by coincidence.
/// </summary>
internal static class TestFormulas
{
    public const decimal BaseDayRental = 500m;
    public const decimal BaseKmPrice = 2m;

    public static readonly DateTimeOffset Pickup = new(2026, 9, 1, 8, 0, 0, TimeSpan.Zero);

    public static BaseRates Rates => new(BaseDayRental, BaseKmPrice);

    public static PriceFormula SmallCar => new(CarCategory.SmallCar, Rates, dayFactor: 1m, kmFactor: 0m);
    public static PriceFormula Combi => new(CarCategory.Combi, Rates, dayFactor: 1.3m, kmFactor: 1m);
    public static PriceFormula Truck => new(CarCategory.Truck, Rates, dayFactor: 1.5m, kmFactor: 1.5m);

    public static IPriceCatalog Catalog() => new ConfiguredPriceCatalog([SmallCar, Combi, Truck]);

    public static Rental OpenRental(
        string bookingNumber = "BK-1",
        PriceFormula? formula = null,
        int odometerAtPickupKm = 10_000)
        => Rental.PickUp(
            new BookingNumber(bookingNumber),
            "ABC123",
            new PersonalIdentityNumber("19800101-1234"),
            formula ?? Combi,
            Pickup,
            odometerAtPickupKm);
}
