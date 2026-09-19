using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PsychicFiesta.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCarRental(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddTransient<ICarRentalService, CarRentalService>();

        return services;
    }
}