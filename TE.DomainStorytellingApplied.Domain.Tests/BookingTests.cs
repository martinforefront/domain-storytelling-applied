using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingTests
{
    [Fact]
    public void GivenAvailableSchedule_WhenReserving_ShouldKeepBookingDetailsAndConfirm()
    {
        // välj ett rum
        var room = new Room(1, "Meeting room");

        // välj en tid
        var period = new TimePeriod(
            new DateTime(2026, 11, 1, 10, 0, 0),
            new DateTime(2026, 11, 1, 11, 0, 0));

        // ange vem som bokar
        var booker = new Booker(1, "Anna Andersson", "anna@example.com");

        // skapa bokning
        var schedule = new RoomSchedule(room);
        var booking = schedule.Reserve(1, booker, period);

        booking.Room.ShouldBe(room);
        booking.Period.ShouldBe(period);
        booking.Booker.ShouldBeSameAs(booker);
        booking.IsConfirmed.ShouldBeTrue();
    }
}
