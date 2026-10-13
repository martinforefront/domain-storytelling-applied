using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingSystemTests
{

    [Fact]
    public void GivenAvailableRoom_WhenBooking_ShouldCreateConfirmAndNotify()
    {
        using var output = new StringWriter();
        var system = new BookingSystem(new NotificationService(output));
        var contactDetails = new ContactDetails("Anna", "anna@example.com");
        var room = new Room(1, "Meeting room");
        var period = CreatePeriod(10, 11);

        var booking = system.Book(contactDetails, room, period);

        booking.ContactDetails.ShouldBe(contactDetails);
        booking.Room.ShouldBeSameAs(room);
        booking.Period.ShouldBe(period);
        booking.IsConfirmed.ShouldBeTrue();
        output.ToString().ShouldBe(
            $"Till: anna@example.com{Environment.NewLine}" +
            $"Din bokning av Meeting room är bekräftad: 2026-11-01 10:00–2026-11-01 11:00.{Environment.NewLine}");
    }

    [Fact]

    public void GivenOccupiedRoom_WhenBooking_ShouldRejectSecondBookingAndNotifyItsContactEmail()
    {
        using var output = new StringWriter();
        var system = new BookingSystem(new NotificationService(output));
        var room = new Room(1, "Meeting room");
        var period = CreatePeriod(10, 12);
        var first = system.Book(new ContactDetails("Anna", "anna@example.com"), room, period);
        output.GetStringBuilder().Clear();

        var second = system.Book(new ContactDetails("Bertil", "bertil@example.com"), room, period);

        first.IsConfirmed.ShouldBeTrue();
        second.IsConfirmed.ShouldBeFalse();
        second.Id.ShouldNotBe(first.Id);
        output.ToString().ShouldBe(
            $"Till: bertil@example.com{Environment.NewLine}" +
            $"Bokningen kunde inte genomföras. Rummet är redan bokat.{Environment.NewLine}");
    }

    [Fact]

    public void GivenAdjacentPeriod_WhenBooking_ShouldConfirmBothWithDifferentIds()
    {
        using var output = new StringWriter();
        var system = new BookingSystem(new NotificationService(output));
        var contactDetails = new ContactDetails("Anna", "anna@example.com");
        var room = new Room(1, "Meeting room");

        var first = system.Book(contactDetails, room, CreatePeriod(10, 11));
        var second = system.Book(contactDetails, room, CreatePeriod(11, 12));

        first.IsConfirmed.ShouldBeTrue();
        second.IsConfirmed.ShouldBeTrue();
        second.Id.ShouldNotBe(first.Id);
    }

    [Fact]

    public void GivenRejectedBooking_WhenBookingItsFreePart_ShouldSucceed()
    {
        using var output = new StringWriter();
        var system = new BookingSystem(new NotificationService(output));
        var contactDetails = new ContactDetails("Anna", "anna@example.com");
        var room = new Room(1, "Meeting room");
        system.Book(contactDetails, room, CreatePeriod(10, 12));
        var rejected = system.Book(contactDetails, room, CreatePeriod(9, 11));

        var booking = system.Book(contactDetails, room, CreatePeriod(9, 10));

        rejected.IsConfirmed.ShouldBeFalse();
        booking.IsConfirmed.ShouldBeTrue();
        booking.Id.ShouldNotBe(rejected.Id);
    }

    [Fact]

    public void GivenNotificationFailure_WhenBooking_ShouldKeepRoomOccupiedAndPropagateFailure()
    {
        using var output = new FailingWriter();
        var system = new BookingSystem(new NotificationService(output));
        var contactDetails = new ContactDetails("Anna", "anna@example.com");
        var room = new Room(1, "Meeting room");
        var period = CreatePeriod(10, 12);

        var exception = Should.Throw<InvalidOperationException>(
            () => system.Book(contactDetails, room, period));

        exception.Message.ShouldBe("Delivery failed.");
        output.Attempts.ShouldBe(1);
        output.FailDelivery = false;

        var second = system.Book(contactDetails, room, period);

        second.IsConfirmed.ShouldBeFalse();
        output.ToString().ShouldContain("Rummet är redan bokat.");
    }

    [Fact]

    public void GivenDifferentRooms_WhenBookingSameTime_ShouldConfirmBoth()
    {
        using var output = new StringWriter();
        var system = new BookingSystem(new NotificationService(output));
        var contactDetails = new ContactDetails("Anna", "anna@example.com");

        var first = system.Book(contactDetails, new Room(1, "Meeting room"), CreatePeriod(10, 12));
        var second = system.Book(contactDetails, new Room(2, "Meeting room"), CreatePeriod(10, 12));

        first.IsConfirmed.ShouldBeTrue();
        second.IsConfirmed.ShouldBeTrue();
    }

    [Fact]

    public void GivenSameRoomIdInDifferentObjects_WhenBookingSameTime_ShouldReject()
    {
        using var output = new StringWriter();
        var system = new BookingSystem(new NotificationService(output));
        var contactDetails = new ContactDetails("Anna", "anna@example.com");
        system.Book(contactDetails, new Room(1, "Meeting room"), CreatePeriod(10, 12));

        var second = system.Book(contactDetails, new Room(1, "Meeting room"), CreatePeriod(10, 12));

        second.IsConfirmed.ShouldBeFalse();
    }

    [Fact]

    public void GivenCancelledBooking_WhenBookingAgain_ShouldReuseFreedTime()
    {
        using var output = new StringWriter();
        var system = new BookingSystem(new NotificationService(output));
        var contactDetails = new ContactDetails("Anna", "anna@example.com");
        var room = new Room(1, "Meeting room");
        var first = system.Book(contactDetails, room, CreatePeriod(10, 12));

        system.Cancel(room.Id, first.Id);
        var second = system.Book(contactDetails, room, CreatePeriod(10, 12));

        first.IsConfirmed.ShouldBeFalse();
        second.IsConfirmed.ShouldBeTrue();
    }

    private sealed class FailingWriter : StringWriter
    {
        public bool FailDelivery { get; set; } = true;
        public int Attempts { get; private set; }

        public override void WriteLine(string? value)
        {
            Attempts++;
            if (FailDelivery)
            {
                throw new InvalidOperationException("Delivery failed.");
            }
            base.WriteLine(value);
        }
    }

    private static TimePeriod CreatePeriod(int startHour, int endHour) => new(
        new DateTime(2026, 11, 1, startHour, 0, 0),
        new DateTime(2026, 11, 1, endHour, 0, 0));
}
