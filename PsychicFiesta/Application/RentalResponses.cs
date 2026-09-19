using PsychicFiesta.Domain;

namespace PsychicFiesta.Application;

public sealed record PickupConfirmation(
    BookingNumber BookingNumber,
    string RegistrationNumber,
    CarCategory CarCategory,
    PersonalIdentityNumber Ssn,
    DateTimeOffset PickedUpAt,
    int OdometerAtPickupKm);