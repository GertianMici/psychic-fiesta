using PsychicFiesta.Pricing;

namespace PsychicFiesta.Tests.Pricing;

public class PriceFormulaTests
{
    // 500 per day, 2 per km, 2 days, 100 km. Every expected value is written out by hand:
    // re-deriving it from the formula under test would only prove the test can multiply.
    [Theory]
    [InlineData("small-car", 1000, 0, 1000)]
    [InlineData("combi", 1300, 200, 1500)]
    [InlineData("truck", 1500, 300, 1800)]
    public void CalculateBreakdown_PerCategory_SplitsAndTotalsCorrectly(
        string categoryCode,
        decimal expectedDayCharge,
        decimal expectedKmCharge,
        decimal expectedTotal)
    {
        RentalPrice price = FormulaFor(categoryCode).CalculateBreakdown(numberOfDays: 2, numberOfKm: 100);

        Assert.Equal(expectedDayCharge, price.DayCharge);
        Assert.Equal(expectedKmCharge, price.KmCharge);
        Assert.Equal(expectedTotal, price.Total);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(10_000)]
    public void CalculateBreakdown_SmallCar_DistanceIsNeverCharged(int numberOfKm)
    {
        RentalPrice price = TestFormulas.SmallCar.CalculateBreakdown(numberOfDays: 2, numberOfKm);

        Assert.Equal(0m, price.KmCharge);
        Assert.Equal(1000m, price.Total);
    }

    [Fact]
    public void CalculateBreakdown_Always_TotalEqualsTheSumOfTheLines()
    {
        RentalPrice price = TestFormulas.Truck.CalculateBreakdown(numberOfDays: 3, numberOfKm: 137);

        Assert.Equal(price.DayCharge + price.KmCharge, price.Total);
    }

    [Fact]
    public void CalculateBreakdown_EchoesTheInputsOntoTheBreakdown()
    {
        RentalPrice price = TestFormulas.Combi.CalculateBreakdown(numberOfDays: 4, numberOfKm: 250);

        Assert.Equal(4, price.RentalDays);
        Assert.Equal(250, price.NumberOfKm);
    }

    [Fact]
    public void CalculatePrice_AgreesWithTheBreakdownTotal()
    {
        Assert.Equal(
            TestFormulas.Truck.CalculateBreakdown(3, 137).Total,
            TestFormulas.Truck.CalculatePrice(3, 137));
    }

    [Fact]
    public void CalculateBreakdown_ZeroDays_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => TestFormulas.Combi.CalculateBreakdown(numberOfDays: 0, numberOfKm: 10));

    [Fact]
    public void CalculateBreakdown_NegativeDistance_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => TestFormulas.Combi.CalculateBreakdown(numberOfDays: 1, numberOfKm: -1));

    [Fact]
    public void Constructor_NegativeDayFactor_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => new PriceFormula(TestCategories.Combi, TestFormulas.Rates, dayFactor: -1m, kmFactor: 1m));

    [Fact]
    public void Constructor_NegativeKmFactor_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => new PriceFormula(TestCategories.Combi, TestFormulas.Rates, dayFactor: 1m, kmFactor: -1m));

    [Fact]
    public void BaseRates_NegativeDayRental_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new BaseRates(-1m, 2m));

    [Fact]
    public void BaseRates_NegativeKmPrice_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new BaseRates(500m, -1m));

    private static PriceFormula FormulaFor(string categoryCode) => categoryCode switch
    {
        "small-car" => TestFormulas.SmallCar,
        "combi" => TestFormulas.Combi,
        "truck" => TestFormulas.Truck,
        _ => throw new ArgumentOutOfRangeException(nameof(categoryCode))
    };
}
