using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using PsychicFiesta.Application;

namespace PsychicFiesta.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryRentalStore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IRentalRepository, InMemoryRentalRepository>();

        return services;
    }

    public static IServiceCollection AddPriceCatalog(this IServiceCollection services, IConfiguration section)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(section);

        services.AddOptions<PriceCatalogOptions>()
            .Bind(section, binder => binder.ErrorOnUnknownConfiguration = true);

        services.TryAddEnumerable(ServiceDescriptor.Singleton<
            IValidateOptions<PriceCatalogOptions>,
            PriceCatalogOptionsValidator>());

        services.TryAddSingleton<IPriceCatalog>(provider =>
            ConfiguredPriceCatalog.FromOptions(provider.GetRequiredService<IOptions<PriceCatalogOptions>>().Value));

        return services;
    }
}