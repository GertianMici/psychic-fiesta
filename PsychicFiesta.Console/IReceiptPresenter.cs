using System.Globalization;
using PsychicFiesta.Application;
using PsychicFiesta.Domain;

namespace PsychicFiesta.Demo;

public interface IReceiptPresenter
{
    void ShowHeading(string text);
    void ShowPickup(PickupConfirmation confirmation);
    void ShowReceipt(RentalReceipt receipt);
    void ShowExpectedFailure(string scenario, Exception exception);
}

public sealed class ConsoleReceiptPresenter : IReceiptPresenter
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
    private const string Currency = "SEK";
    private const int Width = 58;

    public void ShowHeading(string text)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', Width));
        Console.WriteLine($"  {text}");
        Console.WriteLine(new string('=', Width));
    }

    public void ShowPickup(PickupConfirmation confirmation)
    {
        Field("Booking", confirmation.BookingNumber.ToString());
        Field("Registration", confirmation.RegistrationNumber);
        Field("Category", Describe(confirmation.CarCategory));
        Field("Customer", confirmation.Ssn.ToString());
        Field("Picked up", Timestamp(confirmation.PickedUpAt));
        Field("Odometer", Distance(confirmation.OdometerAtPickupKm));
    }

    public void ShowReceipt(RentalReceipt receipt)
    {
        Field("Returned", Timestamp(receipt.ReturnedAt));
        Field("Odometer", Distance(receipt.OdometerAtReturnKm));
        Rule();
        Field("Billable days", receipt.NumberOfDays.ToString(Culture));
        Field("Distance driven", Distance(receipt.NumberOfKm));
        Field("Day charge", Money(receipt.Pricing.DayCharge));
        Field("Distance charge", Money(receipt.Pricing.KmCharge));
        Rule();
        Field("TOTAL", Money(receipt.Price));
        Console.WriteLine();
    }

    public void ShowExpectedFailure(string scenario, Exception exception)
    {
        Console.WriteLine($"  {scenario}");
        Console.WriteLine($"  rejected: {exception.GetType().Name}");
        Console.WriteLine($"            {exception.Message}");
        Console.WriteLine();
    }

    private static void Field(string label, string value) => Console.WriteLine($"  {label,-17}{value}");

    private static void Rule() => Console.WriteLine($"  {new string('-', Width - 4)}");

    private static string Money(decimal amount) => string.Create(Culture, $"{amount,10:N2} {Currency}");

    private static string Distance(int km) => string.Create(Culture, $"{km,10:N0} km");

    private static string Timestamp(DateTimeOffset moment) => moment.ToString("yyyy-MM-dd HH:mm zzz", Culture);

    private static string Describe(CarCategory category)
        => Culture.TextInfo.ToTitleCase(category.Code.Replace('-', ' '));
}