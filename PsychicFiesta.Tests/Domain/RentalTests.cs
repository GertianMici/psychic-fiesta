using PsychicFiesta.Domain;

namespace PsychicFiesta.Tests.Domain;

public class RentalTests
{
    [Fact]
    public void PickUp_NewRental_IsOpenWithNoReceipt()
    {
        Rental rental = TestFormulas.OpenRental();

        Assert.False(rental.IsReturned);
        Assert.Null(rental.Receipt);
        Assert.Equal(0, rental.Version);
    }

    [Fact]
    public void PickUp_ExposesTheCategoryOfItsFormula()
        => Assert.Equal(CarCategory.Combi, TestFormulas.OpenRental().CarCategory);

    [Fact]
    public void PickUp_TrimsTheRegistrationNumber()
    {
        Rental rental = Rental.PickUp(
            new BookingNumber("BK-1"),
            "  ABC123  ",
            new PersonalIdentityNumber("19800101-1234"),
            TestFormulas.Combi,
            TestFormulas.Pickup,
            odometerAtPickupKm: 0);

        Assert.Equal("ABC123", rental.RegistrationNumber);
    }

    [Fact]
    public void PickUp_NegativeOdometerReading_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => TestFormulas.OpenRental(odometerAtPickupKm: -1));

    [Fact]
    public void Return_DerivesDaysAndDistanceFromTheReadings()
    {
        RentalReturn completed = TestFormulas.OpenRental()
            .Return(TestFormulas.Pickup.AddDays(2), odometerAtReturnKm: 10_100);

        Assert.Equal(2, completed.Receipt.NumberOfDays);
        Assert.Equal(100, completed.Receipt.NumberOfKm);
        Assert.Equal(1500m, completed.Receipt.Price);
    }

    [Fact]
    public void Return_CopiesThePickupDetailsOntoTheReceipt()
    {
        Rental rental = TestFormulas.OpenRental();
        DateTimeOffset returnedAt = TestFormulas.Pickup.AddDays(2);

        RentalReceipt receipt = rental.Return(returnedAt, 10_100).Receipt;

        Assert.Equal(rental.BookingNumber, receipt.BookingNumber);
        Assert.Equal(rental.RegistrationNumber, receipt.RegistrationNumber);
        Assert.Equal(rental.CarCategory, receipt.CarCategory);
        Assert.Equal(rental.PickedUpAt, receipt.PickedUpAt);
        Assert.Equal(returnedAt, receipt.ReturnedAt);
        Assert.Equal(10_000, receipt.OdometerAtPickupKm);
        Assert.Equal(10_100, receipt.OdometerAtReturnKm);
    }

    [Fact]
    public void Return_DoesNotMutateTheRentalItWasCalledOn()
    {
        Rental original = TestFormulas.OpenRental();

        RentalReturn completed = original.Return(TestFormulas.Pickup.AddDays(2), 10_100);

        Assert.False(original.IsReturned);
        Assert.True(completed.Rental.IsReturned);
        Assert.Equal(original.Version + 1, completed.Rental.Version);
    }

    [Fact]
    public void Return_Twice_Throws()
    {
        Rental returned = TestFormulas.OpenRental()
            .Return(TestFormulas.Pickup.AddDays(1), 10_050)
            .Rental;

        Assert.Throws<RentalAlreadyReturnedException>(
            () => returned.Return(TestFormulas.Pickup.AddDays(2), 10_100));
    }

    [Fact]
    public void Return_OdometerBelowThePickupReading_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => TestFormulas.OpenRental().Return(TestFormulas.Pickup.AddDays(1), odometerAtReturnKm: 9_999));

    [Fact]
    public void Return_BeforePickup_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => TestFormulas.OpenRental().Return(TestFormulas.Pickup.AddMinutes(-1), 10_000));

    [Fact]
    public void Return_UnchangedOdometer_ChargesForDaysOnly()
    {
        RentalReturn completed = TestFormulas.OpenRental()
            .Return(TestFormulas.Pickup.AddDays(2), odometerAtReturnKm: 10_000);

        Assert.Equal(0, completed.Receipt.NumberOfKm);
        Assert.Equal(0m, completed.Receipt.Pricing.KmCharge);
        Assert.Equal(1300m, completed.Receipt.Price);
    }

    [Fact]
    public void Return_UsesTheFormulaCapturedAtPickup()
    {
        // The receipt carries the formula that was in force when the car went out, so
        // changing the price list cannot reprice a rental that is already open.
        RentalReturn completed = TestFormulas.OpenRental()
            .Return(TestFormulas.Pickup.AddDays(1), 10_010);

        Assert.Equal(TestFormulas.Combi, completed.Receipt.Formula);
    }
}
