using System.Collections.Concurrent;
using PsychicFiesta.Application;
using PsychicFiesta.Domain;

namespace PsychicFiesta.Infrastructure;

public sealed class InMemoryRentalRepository : IRentalRepository
{
    private readonly ConcurrentDictionary<BookingNumber, Rental> _rentals = new();

    public void AddRental(Rental rental)
    {
        ArgumentNullException.ThrowIfNull(rental);

        if (rental.IsReturned)
        {
            throw new ArgumentException("A new rental must be open.", nameof(rental));
        }

        if (!_rentals.TryAdd(rental.BookingNumber, rental))
        {
            throw new DuplicateBookingException(rental.BookingNumber);
        }
    }

    public Rental? FindRental(BookingNumber bookingNumber)
    {
        ArgumentNullException.ThrowIfNull(bookingNumber);
        return _rentals.GetValueOrDefault(bookingNumber);
    }

    public void UpdateRental(Rental rental)
    {
        ArgumentNullException.ThrowIfNull(rental);

        Rental current = FindRental(rental.BookingNumber)
                         ?? throw new RentalNotFoundException(rental.BookingNumber);

        if (current.Version != rental.Version -1
            || !_rentals.TryUpdate(rental.BookingNumber, rental, current))
        {
            throw new RentalConcurrencyException(rental.BookingNumber);
        }
    }
}