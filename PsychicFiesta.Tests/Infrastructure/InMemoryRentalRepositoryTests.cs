using PsychicFiesta.Application;
using PsychicFiesta.Domain;
using PsychicFiesta.Infrastructure;

namespace PsychicFiesta.Tests.Infrastructure;

public class InMemoryRentalRepositoryTests
{
    private readonly IRentalRepository _repository = new InMemoryRentalRepository();

    [Fact]
    public void AddRental_ThenFindRental_ReturnsTheSameRental()
    {
        Rental rental = TestFormulas.OpenRental();

        _repository.AddRental(rental);

        Assert.Same(rental, _repository.FindRental(new BookingNumber("BK-1")));
    }

    [Fact]
    public void FindRental_MatchesTheBookingNumberRegardlessOfCasing()
    {
        _repository.AddRental(TestFormulas.OpenRental("bk-1"));

        Assert.NotNull(_repository.FindRental(new BookingNumber("BK-1")));
    }

    [Fact]
    public void AddRental_SameBookingNumberTwice_Throws()
    {
        _repository.AddRental(TestFormulas.OpenRental());

        Assert.Throws<DuplicateBookingException>(() => _repository.AddRental(TestFormulas.OpenRental()));
    }

    [Fact]
    public void AddRental_AnAlreadyReturnedRental_Throws()
    {
        Rental returned = TestFormulas.OpenRental().Return(TestFormulas.Pickup.AddDays(1), 10_050).Rental;

        Assert.Throws<ArgumentException>(() => _repository.AddRental(returned));
    }

    [Fact]
    public void FindRental_UnknownBookingNumber_ReturnsNull()
        => Assert.Null(_repository.FindRental(new BookingNumber("NOPE")));

    [Fact]
    public void UpdateRental_StoresTheReturnedRental()
    {
        Rental open = TestFormulas.OpenRental();
        _repository.AddRental(open);

        Rental returned = open.Return(TestFormulas.Pickup.AddDays(1), 10_050).Rental;
        _repository.UpdateRental(returned);

        Assert.Same(returned, _repository.FindRental(open.BookingNumber));
    }

    [Fact]
    public void UpdateRental_WhenAnotherAgentAlreadyReturnedTheCar_Throws()
    {
        Rental open = TestFormulas.OpenRental();
        _repository.AddRental(open);

        // Two agents both load the same open rental and both register a return.
        Rental firstAgent = open.Return(TestFormulas.Pickup.AddDays(1), 10_050).Rental;
        Rental secondAgent = open.Return(TestFormulas.Pickup.AddDays(2), 10_100).Rental;

        _repository.UpdateRental(firstAgent);

        Assert.Throws<RentalConcurrencyException>(() => _repository.UpdateRental(secondAgent));
    }

    [Fact]
    public void UpdateRental_LeavesTheWinningVersionInPlaceAfterALostRace()
    {
        Rental open = TestFormulas.OpenRental();
        _repository.AddRental(open);

        Rental firstAgent = open.Return(TestFormulas.Pickup.AddDays(1), 10_050).Rental;
        Rental secondAgent = open.Return(TestFormulas.Pickup.AddDays(2), 10_100).Rental;

        _repository.UpdateRental(firstAgent);
        Assert.Throws<RentalConcurrencyException>(() => _repository.UpdateRental(secondAgent));

        Assert.Same(firstAgent, _repository.FindRental(open.BookingNumber));
    }

    [Fact]
    public void UpdateRental_UnknownBookingNumber_Throws()
    {
        Rental returned = TestFormulas.OpenRental("BK-404")
            .Return(TestFormulas.Pickup.AddDays(1), 10_050)
            .Rental;

        Assert.Throws<RentalNotFoundException>(() => _repository.UpdateRental(returned));
    }
}
