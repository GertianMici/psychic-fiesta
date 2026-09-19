using PsychicFiesta.Domain;

namespace PsychicFiesta.Application;

public sealed record PickupRequest
{
    public string BookingNumber { get; }
    public string RegistrationNumber { get; }
    public PersonalIdentityNumber Ssn { get; }
    public string CategoryCode { get; }
    public DateTimeOffset PickedUpAt { get; }
    public int OdometerReadKm { get; }

    public PickupRequest(
        string bookingNumber,
        string registrationNumber,
        string ssn,
        string categoryCode,
        DateTimeOffset pickedUpAt,
        int odometerReadKm)
    {
        BookingNumber = bookingNumber;
        RegistrationNumber = registrationNumber;
        Ssn = new PersonalIdentityNumber(ssn);
        CategoryCode = categoryCode;
        PickedUpAt = pickedUpAt;
        OdometerReadKm = odometerReadKm;
    }
}

public sealed record ReturnRequest(
    string BookingNumber,
    DateTimeOffset ReturnedAt,
    int OdometerReadKm);