# Psychic Fiesta

Business logic for a car rental: registering pickups and returns and pricing the rental period, with a scripted console demo. It is an exercise that runs locally; there is no UI or server.

This file is what an agent needs to work on this repo. It holds facts about this repo only; my working agreement holds the rules for every repo.

## Layout

- .NET 10, solution `PsychicFiesta.slnx`.
- `PsychicFiesta/` the core, with no dependencies: `Domain/`, `Application/` (use cases and ports), `Pricing/`.
- `PsychicFiesta.Infrastructure/` the in-memory rental store, the configured price catalog and its options validator, and the DI registration.
- `PsychicFiesta.Console/` the composition root and the scripted demo. Its `appsettings.json` holds the base rates and the car categories.
- `PsychicFiesta.Tests/` xUnit tests for the domain, application, pricing and infrastructure.
- The README holds the pricing formulas and the numbered assumptions (a started day is a whole day, per-line rounding, the price formula captured at pickup). Read it before changing pricing.

## Build, test and run

```bash
dotnet restore PsychicFiesta.slnx
dotnet build PsychicFiesta.slnx --configuration Release
dotnet test PsychicFiesta.slnx --configuration Release
cd PsychicFiesta.Console && dotnet run
```

- The `verify` skill covers the build and the tests.

## Deploy

Nothing deploys. `.github/workflows/build.yml` builds and tests on pushes and pull requests to `main`.

## Data sources

None. Rentals live in memory (`InMemoryRentalRepository`) behind `IRentalRepository`. Prices come from the `PriceCatalog` section of `PsychicFiesta.Console/appsettings.json`.

## Conventions

- Car categories are data, not an enum: a code with a day factor and a km factor, loaded from configuration. Adding a category is a config change.
- Storage sits behind `IRentalRepository` and pricing behind `IPriceCatalog`. Names follow the specification: price formula, base rates, car category.
- Catalog options are validated on start (`PriceCatalogOptionsValidator`).

## Known traps

None recorded yet.
