using System.Collections.Frozen;
using PsychicFiesta.Application;
using PsychicFiesta.Domain;
using PsychicFiesta.Pricing;

namespace PsychicFiesta.Infrastructure;

public sealed class ConfiguredPriceCatalog : IPriceCatalog
{
    private readonly FrozenDictionary<CarCategory, PriceFormula> _formulas;

    public ConfiguredPriceCatalog(IEnumerable<PriceFormula> formulas)
    {
        ArgumentNullException.ThrowIfNull(formulas);
        Dictionary<CarCategory, PriceFormula> byCarCategory = new();

        foreach (PriceFormula formula in formulas)
        {
            ArgumentNullException.ThrowIfNull(formula);

            if (!byCarCategory.TryAdd(formula.CarCategory, formula))
            {
                throw new ArgumentException(
                    $"The formula for {formula.CarCategory} already exists.",
                    nameof(formulas));
            }
        }

        _formulas = byCarCategory.ToFrozenDictionary();
    }

    public PriceFormula GetFormula(CarCategory carCategory)
    {
        ArgumentNullException.ThrowIfNull(carCategory);

        return !_formulas.TryGetValue(carCategory, out PriceFormula? formula)
            ? throw new UnsupportedCategoryException(carCategory)
            : formula;
    }

    public static ConfiguredPriceCatalog FromOptions(PriceCatalogOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        BaseRates catalogRates = ToRates(options.BaseRates);

        return new ConfiguredPriceCatalog(options.Categories.Select(category => new PriceFormula(
            new CarCategory(category.Code),
            category.Rates is null ? catalogRates : ToRates(category.Rates),
            category.DayFactor,
            category.KmFactor)));
    }

    private static BaseRates ToRates(RateOptions rates) =>
        new(rates.BaseDayRental, rates.BaseKmPrice);
}