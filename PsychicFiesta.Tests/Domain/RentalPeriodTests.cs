using PsychicFiesta.Domain;

namespace PsychicFiesta.Tests.Domain;

public class RentalPeriodTests
{
    private static readonly DateTimeOffset PickedUpAt = new(2026, 9, 1, 8, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(0, 30, 1)]    // half an hour still costs a full day
    [InlineData(24, 0, 1)]    // exactly one day
    [InlineData(24, 1, 2)]    // one minute over rolls into a second day
    [InlineData(72, 0, 3)]    // exactly three days
    [InlineData(72, 1, 4)]    // and one minute over rolls into a fourth
    [InlineData(240, 0, 10)]  // ten days
    public void BillableDays_AStartedDayCountsAsAWholeDay(int hours, int minutes, int expectedDays)
    {
        DateTimeOffset returnedAt = PickedUpAt.AddHours(hours).AddMinutes(minutes);

        Assert.Equal(expectedDays, RentalPeriod.BillableDays(PickedUpAt, returnedAt));
    }

    [Fact]
    public void BillableDays_SameInstant_IsOneDay()
        => Assert.Equal(1, RentalPeriod.BillableDays(PickedUpAt, PickedUpAt));

    [Fact]
    public void BillableDays_DifferentOffsets_ComparesAbsoluteInstants()
    {
        // 11:00+02:00 is 09:00Z, one hour after pickup rather than three.
        DateTimeOffset returnedAt = new(2026, 9, 1, 11, 0, 0, TimeSpan.FromHours(2));

        Assert.Equal(1, RentalPeriod.BillableDays(PickedUpAt, returnedAt));
    }

    [Fact]
    public void BillableDays_AcrossAMonthBoundary_CountsElapsedDays()
    {
        DateTimeOffset pickedUpAt = new(2026, 9, 29, 8, 0, 0, TimeSpan.Zero);
        DateTimeOffset returnedAt = new(2026, 10, 2, 8, 0, 0, TimeSpan.Zero);

        Assert.Equal(3, RentalPeriod.BillableDays(pickedUpAt, returnedAt));
    }

    [Fact]
    public void BillableDays_ReturnedBeforePickup_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => RentalPeriod.BillableDays(PickedUpAt, PickedUpAt.AddMinutes(-1)));
}
