namespace PsychicFiesta.Application;

public sealed record PickupRequest(
    string BookingNumber,
    string RegistrationNumber,
    string Ssn,
    string CategoryCode,
    DateTimeOffset PickedUpAt,
    int OdometerReadKm);

public sealed record ReturnRequest(
    string BookingNumber,
    DateTimeOffset ReturnedAt,
    int OdometerReadKm);