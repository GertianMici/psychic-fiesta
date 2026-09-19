using PsychicFiesta.Domain;

namespace PsychicFiesta.Application;

public interface IRentalRepository
{
    /// <exception cref="DuplicateBookingException">Booking number already exists.</exception>
    void AddRental(Rental rental);

    Rental? FindRental(BookingNumber bookingNumber);

    /// <exception cref="RentalNotFoundException">Rental does not exist.</exception>
    /// <exception cref="RentalConcurrencyException">The stored rental has changed since it was loaded.</exception>
    void UpdateRental(Rental rental);
}