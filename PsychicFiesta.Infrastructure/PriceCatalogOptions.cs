using System.Text.Json;
using System.Text.Json.Serialization;
using PsychicFiesta.Domain;
using PsychicFiesta.Pricing;

namespace PsychicFiesta.Infrastructure;

public static class TariffConfiguration
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    public static ConfiguredPriceFormulaCatalog FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        CatalogSettings configuration = JsonSerializer.Deserialize<CatalogSettings>(json, Options)
                                        ?? throw new JsonException("A tariff catalog is required");

        if (configuration.BasRates is null || configuration.Categories is null)
        {
            throw new JsonException("BaseRates and categories cannot be null.");
        }

        BaseRates baseRates = configuration.BasRates.ToRates();

        IEnumerable<PriceFormula> tariffs = configuration.Categories.Select(category => category is null
            ? throw new JsonException("Category cannot be null")
            : new PriceFormula(
                new CarCategory(category.Code),
                category.Rates?.ToRates() ?? baseRates,
                category.DayFactor,
                category.KmFactor));

        return new ConfiguredPriceFormulaCatalog(tariffs);
    }

    private sealed class CatalogSettings
    {
        public required RateSettings BasRates { get; init; }
        public required CategorySettings[] Categories { get; init; }
    }

    private sealed class RateSettings
    {
        public required decimal BaseDayRental { get; init; }
        public required decimal BaseKmPrice { get; init; }
        public BaseRates ToRates() => new(BaseDayRental, BaseKmPrice);
    }

    private sealed class CategorySettings
    {
        public required string Code { get; init; }
        public required decimal DayFactor { get; init; }
        public required decimal KmFactor { get; init; }
        public RateSettings? Rates { get; init; }
    }
}