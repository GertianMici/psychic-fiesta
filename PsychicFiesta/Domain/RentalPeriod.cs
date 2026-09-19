namespace PsychicFiesta.Domain;

public static class RentalPeriod
{
    public static int BillableDays(DateTimeOffset pickedUpAt, DateTimeOffset returnedAt)
    {
        if (returnedAt < pickedUpAt)
        {
            throw new ArgumentOutOfRangeException(
                nameof(returnedAt),
                message: "Return time cannot be earlier than pickup time");
        }

        long ticks = (returnedAt - pickedUpAt).Ticks;
        long wholeDays = (ticks / TimeSpan.TicksPerDay);
        int partialDay = (ticks % TimeSpan.TicksPerDay) == 0 ? 0 : 1;
        return Math.Max(1, checked((int)(wholeDays + partialDay)));
    }
}