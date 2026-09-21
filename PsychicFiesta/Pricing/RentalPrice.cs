namespace PsychicFiesta.Pricing;

public sealed record RentalPrice(
    int RentalDays,
    int NumberOfKm,
    decimal DayCharge,
    decimal KmCharge)
{
    public decimal Total => DayCharge + KmCharge;
}