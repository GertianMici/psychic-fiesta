using PsychicFiesta.Domain;

namespace PsychicFiesta.Tests.Domain;

public class BookingNumberTests
{
    [Theory]
    [InlineData("ab-1")]
    [InlineData("AB-1")]
    [InlineData("  ab-1  ")]
    public void Constructor_NormalisesCaseAndWhitespace(string input)
        => Assert.Equal(new BookingNumber("AB-1"), new BookingNumber(input));

    [Fact]
    public void EqualValues_ShareAHashCode()
        => Assert.Equal(new BookingNumber("AB-1").GetHashCode(), new BookingNumber("ab-1").GetHashCode());

    [Fact]
    public void ToString_ReturnsTheNormalisedValue()
        => Assert.Equal("AB-1", new BookingNumber("  ab-1 ").ToString());

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_BlankValue_Throws(string input)
        => Assert.Throws<ArgumentException>(() => new BookingNumber(input));
}

public class CarCategoryTests
{
    [Theory]
    [InlineData("Combi")]
    [InlineData("combi")]
    [InlineData("  COMBI ")]
    public void Constructor_NormalisesCaseAndWhitespace(string input)
        => Assert.Equal(TestCategories.Combi, new CarCategory(input));

    [Fact]
    public void EqualCodes_ShareAHashCode()
        => Assert.Equal(TestCategories.SmallCar.GetHashCode(), new CarCategory("SMALL-CAR").GetHashCode());

    [Fact]
    public void TheThreeCategories_AreDistinct()
    {
        HashSet<CarCategory> categories = [TestCategories.SmallCar, TestCategories.Combi, TestCategories.Truck];

        Assert.Equal(3, categories.Count);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_BlankCode_Throws(string input)
        => Assert.Throws<ArgumentException>(() => new CarCategory(input));
}

public class PersonalIdentityNumberTests
{
    [Fact]
    public void ToString_MasksTheLastFourDigits()
        => Assert.Equal("19800101-****", new PersonalIdentityNumber("19800101-1234").ToString());

    [Fact]
    public void ToString_AShortValue_IsMaskedEntirely()
        => Assert.Equal("****", new PersonalIdentityNumber("1234").ToString());

    [Fact]
    public void GetRawValue_ReturnsTheUnmaskedNumber()
        => Assert.Equal("19800101-1234", new PersonalIdentityNumber(" 19800101-1234 ").GetRawValue());

    [Fact]
    public void Equality_IgnoresSurroundingWhitespace()
        => Assert.Equal(
            new PersonalIdentityNumber("19800101-1234"),
            new PersonalIdentityNumber(" 19800101-1234 "));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_BlankValue_Throws(string input)
        => Assert.Throws<ArgumentException>(() => new PersonalIdentityNumber(input));
}
