using PsychicFiesta.Application;
using PsychicFiesta.Domain;

namespace PsychicFiesta.Demo;

public sealed class RentalDemo
{
    private static readonly DateTimeOffset Monday = new(2026, 9, 7, 8, 0, 0, TimeSpan.Zero);
    private const string DemoSsn = "19990101-1234";

    private readonly ICarRentalService _rentals;
    private readonly IReceiptPresenter _presenter;

    public RentalDemo(ICarRentalService rentals, IReceiptPresenter presenter)
    {
        ArgumentNullException.ThrowIfNull(rentals);
        ArgumentNullException.ThrowIfNull(presenter);

        _rentals = rentals;
        _presenter = presenter;
    }

    public int Run()
    {
        try
        {
            SmallCarIgnoresDistance();
            CombiChargesForDistance();
            TruckRoundsUpAStartedDay();
            CategoryCodesAreCaseInsensitive();
            ReturningAnUnknownBooking();
            ReturningTheSameCarTwice();
            RentingACategoryWithNoPrice();

            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Unexpected failure: {exception}");
            return 1;
        }
    }

    private void SmallCarIgnoresDistance()
    {
        _presenter.ShowHeading("Small car — 3 days, 500 km driven (distance is not charged)");

        Rent("SC-1001", "ABC 123", "small-car",
            Monday, Monday.AddDays(3), odometerAtPickup: 10_000, odometerAtReturn: 10_500);
    }

    private void CombiChargesForDistance()
    {
        _presenter.ShowHeading("Combi — 2 days, 100 km driven");

        Rent("CB-2002", "DEF 456", "combi",
            Monday, Monday.AddDays(2), odometerAtPickup: 42_000, odometerAtReturn: 42_100);
    }

    private void TruckRoundsUpAStartedDay()
    {
        _presenter.ShowHeading("Truck — out for 1 day and 2 hours, billed as 2 days");

        Rent("TR-3003", "GHI 789", "truck",
            Monday, Monday.AddDays(1).AddHours(2), odometerAtPickup: 88_000, odometerAtReturn: 88_250);
    }

    private void CategoryCodesAreCaseInsensitive()
    {
        _presenter.ShowHeading("Codes are normalised — '  Small-Car  ' resolves to the same category");

        Rent("SC-1002", "JKL 012", "  Small-Car  ",
            Monday, Monday.AddDays(1), odometerAtPickup: 5_000, odometerAtReturn: 5_060);
    }

    private void ReturningAnUnknownBooking()
    {
        _presenter.ShowHeading("Rejected — returning a booking that was never picked up");

        Expect("Return booking NOPE-9999",
            () => _rentals.RegisterReturn(new ReturnRequest("NOPE-9999", Monday.AddDays(1), 1)));
    }

    private void ReturningTheSameCarTwice()
    {
        _presenter.ShowHeading("Rejected — returning the same car twice");

        _rentals.RegisterPickup(new PickupRequest("CB-2003", "MNO 345", DemoSsn, "combi", Monday, 7_000));
        _rentals.RegisterReturn(new ReturnRequest("CB-2003", Monday.AddDays(1), 7_080));

        Expect("Return booking CB-2003 a second time",
            () => _rentals.RegisterReturn(new ReturnRequest("CB-2003", Monday.AddDays(2), 7_150)));
    }

    private void RentingACategoryWithNoPrice()
    {
        _presenter.ShowHeading("Rejected — a category that is not in the price list");

        Expect("Pick up a 'limousine'",
            () => _rentals.RegisterPickup(
                new PickupRequest("LM-5005", "PQR 678", DemoSsn, "limousine", Monday, 0)));
    }

    private void Rent(
        string bookingNumber,
        string registrationNumber,
        string categoryCode,
        DateTimeOffset pickedUpAt,
        DateTimeOffset returnedAt,
        int odometerAtPickup,
        int odometerAtReturn)
    {
        PickupConfirmation confirmation = _rentals.RegisterPickup(new PickupRequest(
            bookingNumber, registrationNumber, DemoSsn, categoryCode, pickedUpAt, odometerAtPickup));

        _presenter.ShowPickup(confirmation);

        RentalReceipt receipt = _rentals.RegisterReturn(
            new ReturnRequest(bookingNumber, returnedAt, odometerAtReturn));

        _presenter.ShowReceipt(receipt);
    }

    private void Expect(string scenario, Action action)
    {
        try
        {
            action();
        }
        catch (InvalidOperationException exception)
        {
            _presenter.ShowExpectedFailure(scenario, exception);
            return;
        }

        throw new InvalidOperationException($"'{scenario}' was expected to be rejected but succeeded.");
    }
}