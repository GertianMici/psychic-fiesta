namespace PsychicFiesta.Domain;

public sealed class DuplicateBookingException(BookingNumber bookingNumber)
    : InvalidOperationException($"Booking {bookingNumber} is already in use.");

public sealed class RentalNotFoundException(BookingNumber bookingNumber)
    : InvalidOperationException($"Booking {bookingNumber} is not found.");

public sealed class RentalAlreadyReturnedException(BookingNumber bookingNumber)
    : InvalidOperationException($"Booking {bookingNumber} has been returned.");

public sealed class RentalConcurrencyException(BookingNumber bookingNumber)
    : InvalidOperationException($"Booking {bookingNumber} changed. Try to reload.");

public sealed class UnsupportedCategoryException(CarCategory category)
    : InvalidOperationException($"No price is configured for category {category}.");