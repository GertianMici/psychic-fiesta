using PsychicFiesta.Pricing;

namespace PsychicFiesta.Domain;

public sealed record RentalReceipt(
    BookingNumber BookingNumber,
    string RegistrationNumber,
    CarCategory CarCategory,
    DateTimeOffset PickedUpAt,
    DateTimeOffset ReturnedAt,
    int OdometerAtPickupKm,
    int OdometerAtReturnKm,
    PriceFormula Formula,
    RentalPrice Pricing)
{
    public int NumberOfDays => Pricing.RentalDays;
    public int NumberOfKm => Pricing.NumberOfKm;
    public decimal Price => Pricing.Total;
}