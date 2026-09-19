using PsychicFiesta.Domain;
using PsychicFiesta.Pricing;

namespace PsychicFiesta.Application;

public interface IPriceCatalog
{
    /// <summary>returns the current immutable terms for rental.</summary>
    /// <exception cref="UnsupportedCategoryException">The category is not supported.</exception>
    PriceFormula GetFormula(CarCategory carCategory);
}