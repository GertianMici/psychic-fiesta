using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PsychicFiesta.Domain;
using PsychicFiesta.Infrastructure;

namespace PsychicFiesta.Tests.Infrastructure;

public class PriceCatalogOptionsTests
{
    private const string Json = """
    {
      "PriceCatalog": {
        "BaseRates": { "BaseDayRental": 500, "BaseKmPrice": 2 },
        "Categories": [
          { "Code": "small-car", "DayFactor": 1.0, "KmFactor": 0.0 },
          { "Code": "combi",     "DayFactor": 1.3, "KmFactor": 1.0 },
          { "Code": "truck",     "DayFactor": 1.5, "KmFactor": 1.5 }
        ]
      }
    }
    """;

    [Fact]
    public void Configuration_BindsAndReproducesTheSpecificationFormulas()
    {
        ConfiguredPriceCatalog catalog = ConfiguredPriceCatalog.FromOptions(Bind(Json));

        Assert.Equal(1000m, catalog.GetFormula(TestCategories.SmallCar).CalculatePrice(2, 100));
        Assert.Equal(1500m, catalog.GetFormula(TestCategories.Combi).CalculatePrice(2, 100));
        Assert.Equal(1800m, catalog.GetFormula(TestCategories.Truck).CalculatePrice(2, 100));
    }

    [Fact]
    public void Configuration_CategoryCodesAreNormalised()
    {
        const string mixedCase = """
        {
          "PriceCatalog": {
            "BaseRates": { "BaseDayRental": 500, "BaseKmPrice": 2 },
            "Categories": [ { "Code": "  Small-Car  ", "DayFactor": 1.0, "KmFactor": 0.0 } ]
          }
        }
        """;

        ConfiguredPriceCatalog catalog = ConfiguredPriceCatalog.FromOptions(Bind(mixedCase));

        Assert.Equal(1000m, catalog.GetFormula(TestCategories.SmallCar).CalculatePrice(2, 100));
    }

    [Fact]
    public void FromOptions_ACategoryMayOverrideTheCatalogRates()
    {
        PriceCatalogOptions options = Catalog(
            new CategoryOptions
            {
                Code = "combi",
                DayFactor = 1m,
                KmFactor = 1m,
                Rates = new RateOptions { BaseDayRental = 100m, BaseKmPrice = 1m }
            });

        ConfiguredPriceCatalog catalog = ConfiguredPriceCatalog.FromOptions(options);

        // 100 * 1 * 2 days + 1 * 1 * 100 km, i.e. the override rather than the 500/2 catalog rates.
        Assert.Equal(300m, catalog.GetFormula(TestCategories.Combi).CalculatePrice(2, 100));
    }

    [Fact]
    public void GetFormula_CategoryNotInTheCatalog_Throws()
    {
        ConfiguredPriceCatalog catalog = ConfiguredPriceCatalog.FromOptions(Bind(Json));

        Assert.Throws<UnsupportedCategoryException>(() => catalog.GetFormula(new CarCategory("limousine")));
    }

    [Fact]
    public void Validator_AValidCatalog_Succeeds()
        => Assert.True(new PriceCatalogOptionsValidator().Validate(null, Bind(Json)).Succeeded);

    [Fact]
    public void Validator_DuplicateCategoryCode_Fails()
    {
        PriceCatalogOptions options = Catalog(
            new CategoryOptions { Code = "combi", DayFactor = 1.3m, KmFactor = 1m },
            new CategoryOptions { Code = "COMBI", DayFactor = 1.4m, KmFactor = 1m });

        ValidateOptionsResult result = new PriceCatalogOptionsValidator().Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures!, failure => failure.Contains("repeats the category code"));
    }

    [Fact]
    public void Validator_NoCategories_Fails()
        => Assert.True(new PriceCatalogOptionsValidator().Validate(null, Catalog()).Failed);

    [Fact]
    public void Validator_BlankCategoryCode_Fails()
    {
        PriceCatalogOptions options = Catalog(
            new CategoryOptions { Code = "   ", DayFactor = 1m, KmFactor = 1m });

        Assert.True(new PriceCatalogOptionsValidator().Validate(null, options).Failed);
    }

    [Fact]
    public void Validator_NegativeFactor_Fails()
    {
        PriceCatalogOptions options = Catalog(
            new CategoryOptions { Code = "combi", DayFactor = -1m, KmFactor = 1m });

        Assert.True(new PriceCatalogOptionsValidator().Validate(null, options).Failed);
    }

    [Fact]
    public void Validator_NegativeBaseRate_Fails()
    {
        PriceCatalogOptions options = new()
        {
            BaseRates = new RateOptions { BaseDayRental = -1m, BaseKmPrice = 2m },
            Categories = [new CategoryOptions { Code = "combi", DayFactor = 1m, KmFactor = 1m }]
        };

        Assert.True(new PriceCatalogOptionsValidator().Validate(null, options).Failed);
    }

    [Fact]
    public void Validator_NegativeRateOnACategoryOverride_Fails()
    {
        PriceCatalogOptions options = Catalog(
            new CategoryOptions
            {
                Code = "combi",
                DayFactor = 1m,
                KmFactor = 1m,
                Rates = new RateOptions { BaseDayRental = 500m, BaseKmPrice = -1m }
            });

        Assert.True(new PriceCatalogOptionsValidator().Validate(null, options).Failed);
    }

    private static PriceCatalogOptions Catalog(params CategoryOptions[] categories) => new()
    {
        BaseRates = new RateOptions { BaseDayRental = 500m, BaseKmPrice = 2m },
        Categories = categories
    };

    private static PriceCatalogOptions Bind(string json)
        => new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)))
            .Build()
            .GetSection(PriceCatalogOptions.SectionName)
            .Get<PriceCatalogOptions>()!;
}
