using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingTests
{
    [Fact]
    public void GivenAvailableSchedule_WhenReserving_ShouldKeepBookingDetailsAndReserve()
    {

        var room = new Room(1, "Meeting room");


        var period = new TimePeriod(
            new DateTime(2026, 11, 1, 10, 0, 0),
            new DateTime(2026, 11, 1, 11, 0, 0));


        var contactDetails = new ContactDetails("Anna Andersson", "anna@example.com");


        var schedule = new RoomSchedule(room);
        var booking = schedule.Reserve(1, contactDetails, period);

        booking.Room.ShouldBe(room);
        booking.Period.ShouldBe(period);
        booking.ContactDetails.ShouldBe(contactDetails);
        booking.Status.ShouldBe(BookingStatus.Reserved);
        booking.IsConfirmed.ShouldBeFalse();
    }
}
