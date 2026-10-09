using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingServiceTests
{
    private readonly BookingService service = new();

    [Theory]
    [InlineData(9, 11)]
    [InlineData(11, 13)]
    [InlineData(10, 12)]
    [InlineData(9, 13)]
    [InlineData(10, 11)]
    public void GivenOverlappingConfirmedBooking_WhenConfirmed_ShouldThrow(int startHour, int endHour)
    {
        var existing = CreateBooking(1, 1, 10, 12);
        existing.Confirm();
        var booking = CreateBooking(2, 1, startHour, endHour);

        Should.Throw<InvalidOperationException>(() => service.Confirm(booking, [existing]));

        booking.IsConfirmed.ShouldBeFalse();
    }

    [Theory]
    [InlineData(8, 9)]
    [InlineData(9, 10)]
    [InlineData(12, 13)]
    [InlineData(13, 14)]
    public void GivenNonOverlappingConfirmedBooking_WhenConfirmed_ShouldSucceed(int startHour, int endHour)
    {
        var existing = CreateBooking(1, 1, 10, 12);
        existing.Confirm();
        var booking = CreateBooking(2, 1, startHour, endHour);

        service.Confirm(booking, [existing]);

        booking.IsConfirmed.ShouldBeTrue();
    }

    [Fact]
    public void GivenDifferentRoom_WhenConfirmed_ShouldSucceed()
    {
        var existing = CreateBooking(1, 1, 10, 12);
        existing.Confirm();
        var booking = CreateBooking(2, 2, 10, 12);

        service.Confirm(booking, [existing]);

        booking.IsConfirmed.ShouldBeTrue();
    }

    [Fact]
    public void GivenUnconfirmedBooking_WhenConfirmed_ShouldSucceed()
    {
        var existing = CreateBooking(1, 1, 10, 12);
        var booking = CreateBooking(2, 1, 10, 12);

        service.Confirm(booking, [existing]);

        booking.IsConfirmed.ShouldBeTrue();
    }

    [Fact]
    public void GivenNoBookings_WhenConfirmed_ShouldSucceed()
    {
        var booking = CreateBooking(1, 1, 10, 12);

        service.Confirm(booking, []);

        booking.IsConfirmed.ShouldBeTrue();
    }

    [Fact]
    public void GivenOwnBookingInList_WhenConfirmedAgain_ShouldSucceed()
    {
        var booking = CreateBooking(1, 1, 10, 12);
        booking.Confirm();

        service.Confirm(booking, [booking]);

        booking.IsConfirmed.ShouldBeTrue();
    }

    private static Booking CreateBooking(int id, int roomId, int startHour, int endHour)
    {
        return new Booking(id, new Room(roomId, "Meeting room"), new TimePeriod(
            new DateTime(2026, 11, 1, startHour, 0, 0),
            new DateTime(2026, 11, 1, endHour, 0, 0)));
    }
}
