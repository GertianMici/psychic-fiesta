namespace PsychicFiesta.Infrastructure;

public sealed class PriceCatalogOptions
{
    public const string SectionName = "PriceCatalog";
    public RateOptions BaseRates { get; set; } = new();
    public CategoryOptions[] Categories { get; set; } = [];
}

public sealed class RateOptions
{
    public decimal BaseDayRental { get; set; }
    public decimal BaseKmPrice { get; set; }
}

public sealed class CategoryOptions
{
    public string Code { get; set; } = string.Empty;
    public decimal DayFactor { get; set; }
    public decimal KmFactor { get; set; }

    public RateOptions? Rates { get; set; }
}