using PsychicFiesta.Pricing;

namespace PsychicFiesta.Domain;

public sealed class Rental
{
    public BookingNumber BookingNumber { get; }
    public string RegistrationNumber { get; }
    public PersonalIdentityNumber Ssn { get; }
    public PriceFormula Formula { get; }
    public CarCategory CarCategory => Formula.CarCategory;
    public DateTimeOffset PickedUpAt { get; }
    public int OdometerAtPickupKm { get; }
    public RentalReceipt? Receipt { get; }
    public bool IsReturned => Receipt is not null;
    public int Version { get; }

    private Rental(
        BookingNumber bookingNumber,
        string registrationNumber,
        PersonalIdentityNumber ssn,
        PriceFormula formula,
        DateTimeOffset pickedUpAt,
        int odometerAtPickupKm)
    {
        BookingNumber = bookingNumber;
        RegistrationNumber = registrationNumber;
        Ssn = ssn;
        Formula = formula;
        PickedUpAt = pickedUpAt;
        OdometerAtPickupKm = odometerAtPickupKm;
    }

    private Rental(Rental original, RentalReceipt receipt)
        : this(
            original.BookingNumber,
            original.RegistrationNumber,
            original.Ssn,
            original.Formula,
            original.PickedUpAt,
            original.OdometerAtPickupKm)
    {
        Receipt = receipt;
        Version = original.Version + 1;
    }

    public static Rental PickUp(
        BookingNumber bookingNumber,
        string registrationNumber,
        PersonalIdentityNumber ssn,
        PriceFormula formula,
        DateTimeOffset pickedUpAt,
        int odometerAtPickupKm)
    {
        ArgumentNullException.ThrowIfNull(bookingNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(registrationNumber);
        ArgumentNullException.ThrowIfNull(ssn);
        ArgumentNullException.ThrowIfNull(formula);
        ArgumentOutOfRangeException.ThrowIfNegative(odometerAtPickupKm);

        return new Rental(
            bookingNumber,
            registrationNumber.Trim(),
            ssn,
            formula,
            pickedUpAt,
            odometerAtPickupKm);
    }

    public RentalReturn Return(DateTimeOffset returnedAt, int odometerAtReturnKm)
    {
        if (IsReturned)
        {
            throw new RentalAlreadyReturnedException(BookingNumber);
        }

        if (odometerAtReturnKm < OdometerAtPickupKm)
        {
            throw new ArgumentOutOfRangeException(
                nameof(odometerAtReturnKm),
                "The odometer at return cannot be lower than the picked up at.!");
        }

        int days = RentalPeriod.BillableDays(PickedUpAt, returnedAt);
        int km = odometerAtReturnKm - OdometerAtPickupKm;
        RentalPrice pricing = Formula.CalculateBreakdown(days, km);

        RentalReceipt receipt = new(
            BookingNumber,
            RegistrationNumber,
            CarCategory,
            PickedUpAt,
            returnedAt,
            OdometerAtPickupKm,
            odometerAtReturnKm,
            Formula,
            pricing);

        return new RentalReturn(new Rental(this, receipt), receipt);
    }
}