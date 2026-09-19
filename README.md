# Car rental

Business logic for registering car pickups and returns, and pricing the rental period.

## Running

    cd PsychicFiesta.Console && dotnet run
    dotnet test

## Design

| Project | Contains | Depends on |
|---|---|---|
| `PsychicFiesta` | Domain model, pricing, use cases, ports | nothing |
| `PsychicFiesta.Infrastructure` | In-memory store, configured price catalog | core |
| `PsychicFiesta.Console` | Composition root and a scripted demo | core + infrastructure |
| `PsychicFiesta.Tests` | Unit tests | core + infrastructure |

Car categories are **data, not an enum**. A category is a code plus a day factor and a km
factor, loaded from configuration, so adding one is a config change rather than a code
change. The specification's three formulas are one formula with different factors:

| Category | Day factor | Km factor | Resulting formula |
|---|---|---|---|
| Small car | 1.0 | 0.0 | `baseDayRental * days` |
| Combi | 1.3 | 1.0 | `baseDayRental * days * 1.3 + baseKmPrice * km` |
| Truck | 1.5 | 1.5 | `baseDayRental * days * 1.5 + baseKmPrice * km * 1.5` |

Storage sits behind `IRentalRepository` and pricing behind `IPriceCatalog`, so swapping the
in-memory store for a database is one line in the composition root. Naming follows the
specification's own vocabulary: *price formula*, *base rates*, *car category*.

## Assumptions

1. **A started day is a whole day**, and a rental is always at least one day. A 25-hour
   rental is billed as two days; a 20-minute rental as one.
2. **Distance is the difference between the two odometer readings.** A return reading below
   the pickup reading is rejected rather than clamped to zero.
3. **Prices are rounded to two decimals, half away from zero, per line.** The day charge and
   the distance charge are each rounded and then summed, so a receipt always adds up.
4. **Booking numbers and category codes are case- and whitespace-insensitive**, normalised
   on construction.
5. **The price formula is captured at pickup.** Changing the price list does not reprice a
   rental that is already open.
6. **One currency, unmodelled.** Prices are bare `decimal`. A `Money` type carrying a
   currency is the obvious extension once a second market exists.
7. **The customer's identity number is stored as given and masked whenever displayed.** No
   checksum validation — the format varies by market.
8. **Out of scope:** fuel, insurance, deposits, late fees, discounts, VAT, cancellations,
   and persistence beyond the process.
