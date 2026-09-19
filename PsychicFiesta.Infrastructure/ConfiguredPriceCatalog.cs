using System.Collections.Frozen;
using PsychicFiesta.Application;
using PsychicFiesta.Domain;
using PsychicFiesta.Pricing;

namespace PsychicFiesta.Infrastructure;

public sealed class ConfiguredPriceFormulaCatalog : IPriceFormulaCatalog
{
    private readonly FrozenDictionary<CarCategory, PriceFormula> _formulaCatalog;

    public ConfiguredPriceFormulaCatalog(IEnumerable<PriceFormula> tariffs)
    {
        ArgumentNullException.ThrowIfNull(tariffs);
        Dictionary<CarCategory, PriceFormula> byCarCategory = new();

        foreach (PriceFormula tariff in tariffs)
        {
            ArgumentNullException.ThrowIfNull(tariff);

            if (!byCarCategory.TryAdd(tariff.CarCategory, tariff))
            {
                throw new ArgumentException(
                    $"The category tariff for {tariff.CarCategory} already exists.",
                    nameof(tariffs));
            }
        }

        _formulaCatalog = byCarCategory.ToFrozenDictionary();
    }

    public PriceFormula GetFormula(CarCategory carCategory)
    {
        ArgumentNullException.ThrowIfNull(carCategory);

        return !_formulaCatalog.TryGetValue(carCategory, out PriceFormula? tariff)
            ? throw new UnsupportedCategoryException(carCategory)
            : tariff;
    }

    public static ConfiguredPriceFormulaCatalog CreateDefault(BaseRates rates) => new(
    [
        new PriceFormula(CarCategory.SmallCar, rates, dayFactor: 1m, kmFactor: 0m),
        new PriceFormula(CarCategory.Combi, rates, dayFactor: 1.3m, kmFactor: 1m),
        new PriceFormula(CarCategory.Truck, rates, dayFactor: 1.5m, kmFactor: 1.5m),
    ]);
}