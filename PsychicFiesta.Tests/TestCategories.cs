using PsychicFiesta.Domain;

namespace PsychicFiesta.Tests;

/// <summary>
/// The three categories the specification names. They live here rather than on CarCategory
/// because a category is data loaded from configuration: nothing in the domain knows that
/// there are exactly three of them.
/// </summary>
internal static class TestCategories
{
    public static CarCategory SmallCar => new("small-car");
    public static CarCategory Combi => new("combi");
    public static CarCategory Truck => new("truck");
}
