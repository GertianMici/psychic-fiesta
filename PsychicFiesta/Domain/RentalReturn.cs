using PsychicFiesta.Domain;

namespace PsychicFiesta.Application;

public sealed record RentalReturn(Rental Rental, RentalReceipt  Receipt);