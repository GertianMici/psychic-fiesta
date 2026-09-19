
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PsychicFiesta.Application;
using PsychicFiesta.Demo;
using PsychicFiesta.Infrastructure;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddCarRental();
builder.Services.AddInMemoryRentalStore();
builder.Services.AddPriceCatalog(builder.Configuration.GetSection(PriceCatalogOptions.SectionName));

builder.Services.AddOptions<PriceCatalogOptions>().ValidateOnStart();

builder.Services.AddSingleton<IReceiptPresenter, ConsoleReceiptPresenter>();
builder.Services.AddSingleton<RentalDemo>();
builder.Logging.SetMinimumLevel(LogLevel.Warning);

using IHost host = builder.Build();

await host.StartAsync();
int exitCode = host.Services.GetRequiredService<RentalDemo>().Run();
await host.StopAsync();

return exitCode;