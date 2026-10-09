using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingTests
{
    [Fact]
    public void GivenANewBooking_WhenConfirmed_ShouldBeConfirmed()
    {
        // 1. Choose a room.
        var room = new Room(1, "Meeting room");

        // 2. Choose a time period.
        var period = new TimePeriod(
            new DateTime(2026, 11, 1, 10, 0, 0),
            new DateTime(2026, 11, 1, 11, 0, 0));

        // 3. Create a booking.
        var booking = new Booking(1, room, period);

        booking.Room.ShouldBe(room);
        booking.Period.ShouldBe(period);
        booking.IsConfirmed.ShouldBeFalse();

        // 4. Confirm the booking.
        booking.Confirm();

        booking.IsConfirmed.ShouldBeTrue();
    }
}
