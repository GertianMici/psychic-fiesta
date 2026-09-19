using PsychicFiesta.Domain;
using PsychicFiesta.Pricing;

namespace PsychicFiesta.Application;

public interface ICarRentalService
{
    PickupConfirmation RegisterPickup(PickupRequest request);
    RentalReceipt RegisterReturn(ReturnRequest request);
}

public sealed class CarRentalService : ICarRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IPriceCatalog _priceCatalog;

    public CarRentalService(IRentalRepository rentalRepository, IPriceCatalog priceCatalog)
    {
        _rentalRepository = rentalRepository;
        _priceCatalog = priceCatalog;
    }


    public PickupConfirmation RegisterPickup(PickupRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        BookingNumber booking = new(request.BookingNumber);
        PersonalIdentityNumber ssn = new(request.Ssn);
        CarCategory category = new(request.CategoryCode);

        PriceFormula formula = _priceCatalog.GetFormula(category);

        Rental rental = Rental.PickUp(
            booking,
            request.RegistrationNumber,
            ssn,
            formula,
            request.PickedUpAt,
            request.OdometerReadKm);

        _rentalRepository.AddRental(rental);

        return new PickupConfirmation(
            rental.BookingNumber,
            rental.RegistrationNumber,
            rental.CarCategory,
            rental.Ssn,
            rental.PickedUpAt,
            rental.OdometerAtPickupKm);
    }

    public RentalReceipt RegisterReturn(ReturnRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        BookingNumber booking = new(request.BookingNumber);

        Rental current = _rentalRepository.FindRental(booking)
                         ?? throw new RentalNotFoundException(booking);

        RentalReturn completed = current.Return(request.ReturnedAt, request.OdometerReadKm);
        _rentalRepository.UpdateRental(completed.Rental);

        return completed.Receipt;
    }
}