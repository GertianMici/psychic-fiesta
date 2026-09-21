using PsychicFiesta.Application;
using PsychicFiesta.Domain;
using PsychicFiesta.Infrastructure;

namespace PsychicFiesta.Tests.Application;

/// <summary>
/// Driven through the real repository and the real price catalog rather than mocks:
/// the design does not need test doubles here, which is worth demonstrating.
/// </summary>
public class CarRentalServiceTests
{
    private readonly ICarRentalService _service =
        new CarRentalService(new InMemoryRentalRepository(), TestFormulas.Catalog());

    private static PickupRequest Pickup(
        string bookingNumber = "BK-1",
        string categoryCode = "combi",
        int odometerReadKm = 10_000)
        => new(bookingNumber, "ABC123", "19800101-1234", categoryCode, TestFormulas.Pickup, odometerReadKm);

    [Fact]
    public void PickupThenReturn_ProducesAPricedReceipt()
    {
        _service.RegisterPickup(Pickup());

        RentalReceipt receipt = _service.RegisterReturn(
            new ReturnRequest("BK-1", TestFormulas.Pickup.AddDays(2), 10_100));

        Assert.Equal(2, receipt.NumberOfDays);
        Assert.Equal(100, receipt.NumberOfKm);
        Assert.Equal(1500m, receipt.Price);
    }

    [Fact]
    public void RegisterPickup_ReturnsAConfirmation_NotTheAggregate()
    {
        PickupConfirmation confirmation = _service.RegisterPickup(Pickup());

        Assert.Equal(new BookingNumber("BK-1"), confirmation.BookingNumber);
        Assert.Equal("ABC123", confirmation.RegistrationNumber);
        Assert.Equal(TestCategories.Combi, confirmation.CarCategory);
        Assert.Equal(TestFormulas.Pickup, confirmation.PickedUpAt);
        Assert.Equal(10_000, confirmation.OdometerAtPickupKm);
    }

    [Fact]
    public void RegisterPickup_MasksTheCustomerIdentityOnTheConfirmation()
        => Assert.Equal("19800101-****", _service.RegisterPickup(Pickup()).Ssn.ToString());

    [Fact]
    public void RegisterPickup_BookingNumberAlreadyInUse_Throws()
    {
        _service.RegisterPickup(Pickup());

        Assert.Throws<DuplicateBookingException>(() => _service.RegisterPickup(Pickup()));
    }

    [Fact]
    public void RegisterPickup_CategoryWithNoConfiguredPrice_Throws()
        => Assert.Throws<UnsupportedCategoryException>(
            () => _service.RegisterPickup(Pickup(categoryCode: "limousine")));

    [Fact]
    public void RegisterPickup_NullRequest_Throws()
        => Assert.Throws<ArgumentNullException>(() => _service.RegisterPickup(null!));

    [Fact]
    public void RegisterReturn_UnknownBookingNumber_Throws()
        => Assert.Throws<RentalNotFoundException>(
            () => _service.RegisterReturn(new ReturnRequest("NOPE", TestFormulas.Pickup, 1)));

    [Fact]
    public void RegisterReturn_Twice_Throws()
    {
        _service.RegisterPickup(Pickup());
        _service.RegisterReturn(new ReturnRequest("BK-1", TestFormulas.Pickup.AddDays(1), 10_050));

        Assert.Throws<RentalAlreadyReturnedException>(
            () => _service.RegisterReturn(new ReturnRequest("BK-1", TestFormulas.Pickup.AddDays(2), 10_100)));
    }

    [Fact]
    public void RegisterReturn_OdometerBelowThePickupReading_Throws()
    {
        _service.RegisterPickup(Pickup());

        Assert.Throws<ArgumentOutOfRangeException>(
            () => _service.RegisterReturn(new ReturnRequest("BK-1", TestFormulas.Pickup.AddDays(1), 9_999)));
    }

    [Fact]
    public void BookingNumbersAndCategoryCodes_AreMatchedRegardlessOfCasing()
    {
        _service.RegisterPickup(Pickup(bookingNumber: "bk-9", categoryCode: "  Small-Car  "));

        RentalReceipt receipt = _service.RegisterReturn(
            new ReturnRequest("BK-9", TestFormulas.Pickup.AddDays(2), 10_500));

        Assert.Equal(TestCategories.SmallCar, receipt.CarCategory);
        Assert.Equal(1000m, receipt.Price); // 2 days x 500; the 500 km are not charged
    }

    [Fact]
    public void EachBooking_IsPricedByItsOwnCategory()
    {
        _service.RegisterPickup(Pickup(bookingNumber: "BK-COMBI", categoryCode: "combi"));
        _service.RegisterPickup(Pickup(bookingNumber: "BK-TRUCK", categoryCode: "truck"));

        RentalReceipt combi = _service.RegisterReturn(
            new ReturnRequest("BK-COMBI", TestFormulas.Pickup.AddDays(2), 10_100));
        RentalReceipt truck = _service.RegisterReturn(
            new ReturnRequest("BK-TRUCK", TestFormulas.Pickup.AddDays(2), 10_100));

        Assert.Equal(1500m, combi.Price);
        Assert.Equal(1800m, truck.Price);
    }
}
