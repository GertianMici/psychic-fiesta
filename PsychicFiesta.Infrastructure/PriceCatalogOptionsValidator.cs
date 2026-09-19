using Microsoft.Extensions.Options;

namespace PsychicFiesta.Infrastructure;

public sealed class PriceCatalogOptionsValidator : IValidateOptions<PriceCatalogOptions>
{
    public ValidateOptionsResult Validate(string? name, PriceCatalogOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        List<string> failures = [];

        ValidateRates(options.BaseRates, nameof(PriceCatalogOptions.BaseRates), failures);

        if (options.Categories.Length == 0)
        {
            failures.Add("At least one car category must be configured.");
            return ValidateOptionsResult.Fail(failures);
        }

        HashSet<string> seenCodes = new(StringComparer.OrdinalIgnoreCase);

        for (int index = 0; index < options.Categories.Length; index++)
        {
            CategoryOptions category = options.Categories[index];
            string path = $"{nameof(PriceCatalogOptions.Categories)}[{index}]";

            if (string.IsNullOrWhiteSpace(category.Code))
            {
                failures.Add($"'{path}.{nameof(CategoryOptions.Code)}' is required.");
            }
            else if (!seenCodes.Add(category.Code.Trim()))
            {
                failures.Add($"'{path}' repeats the category code '{category.Code.Trim()}'.");
            }

            if (category.DayFactor < 0)
            {
                failures.Add($"'{path}.{nameof(CategoryOptions.DayFactor)}' cannot be negative.");
            }

            if (category.KmFactor < 0)
            {
                failures.Add($"'{path}.{nameof(CategoryOptions.KmFactor)}' cannot be negative.");
            }

            if (category.Rates is not null)
            {
                ValidateRates(category.Rates, $"{path}.{nameof(CategoryOptions.Rates)}", failures);
            }
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static void ValidateRates(RateOptions rates, string path, List<string> failures)
    {
        if (rates.BaseDayRental < 0)
        {
            failures.Add($"'{path}.{nameof(RateOptions.BaseDayRental)}' cannot be negative.");
        }

        if (rates.BaseKmPrice < 0)
        {
            failures.Add($"'{path}.{nameof(RateOptions.BaseKmPrice)}' cannot be negative.");
        }
    }
}