using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class RoomScheduleTests
{
    private readonly Booker booker = new(1, "Anna", "anna@example.com");

    [Theory]
    [InlineData(9, 11)]
    [InlineData(11, 13)]
    [InlineData(10, 12)]
    [InlineData(9, 13)]
    [InlineData(10, 11)]
    public void GivenOverlappingBooking_WhenReserving_ShouldReject(int startHour, int endHour)
    {
        var schedule = new RoomSchedule(new Room(1, "Meeting room"));
        var existing = schedule.Reserve(1, booker, Period(10, 12));

        Should.Throw<RoomUnavailableException>(
            () => schedule.Reserve(2, booker, Period(startHour, endHour)));

        existing.Status.ShouldBe(BookingStatus.Reserved);
    }

    [Theory]
    [InlineData(8, 9)]
    [InlineData(9, 10)]
    [InlineData(12, 13)]
    [InlineData(13, 14)]
    public void GivenNonOverlappingBooking_WhenReserving_ShouldReserve(int startHour, int endHour)
    {
        var schedule = new RoomSchedule(new Room(1, "Meeting room"));
        schedule.Reserve(1, booker, Period(10, 12));

        var booking = schedule.Reserve(2, booker, Period(startHour, endHour));

        booking.Status.ShouldBe(BookingStatus.Reserved);
    }

    [Fact]
    public void GivenCancelledBooking_WhenReservingSameTime_ShouldReserve()
    {
        var schedule = new RoomSchedule(new Room(1, "Meeting room"));
        var first = schedule.Reserve(1, booker, Period(10, 12));
        schedule.Cancel(first.Id);

        var second = schedule.Reserve(2, booker, Period(10, 12));

        first.Status.ShouldBe(BookingStatus.Cancelled);
        first.IsConfirmed.ShouldBeFalse();
        second.Status.ShouldBe(BookingStatus.Reserved);
        Should.Throw<RoomUnavailableException>(
            () => schedule.Reserve(3, booker, Period(10, 12)));
    }

    [Fact]
    public void GivenDuplicateId_WhenReservingFreeTime_ShouldReject()
    {
        var schedule = new RoomSchedule(new Room(1, "Meeting room"));
        schedule.Reserve(1, booker, Period(10, 12));

        Should.Throw<ArgumentException>(() => schedule.Reserve(1, booker, Period(12, 13)));

        schedule.Reserve(2, booker, Period(12, 13)).Status.ShouldBe(BookingStatus.Reserved);
    }

    [Fact]
    public void GivenUnknownBooking_WhenCancelling_ShouldNotFreeExistingTime()
    {
        var schedule = new RoomSchedule(new Room(1, "Meeting room"));
        schedule.Reserve(1, booker, Period(10, 12));

        Should.Throw<KeyNotFoundException>(() => schedule.Cancel(2));
        Should.Throw<RoomUnavailableException>(() => schedule.Reserve(2, booker, Period(10, 12)));
    }

    [Fact]
    public void GivenReservedBooking_WhenConfirming_ShouldConfirmAndKeepTimeOccupied()
    {
        var schedule = new RoomSchedule(new Room(1, "Meeting room"));
        var booking = schedule.Reserve(1, booker, Period(10, 12));

        schedule.Confirm(booking.Id);

        booking.Status.ShouldBe(BookingStatus.Confirmed);
        booking.IsConfirmed.ShouldBeTrue();
        Should.Throw<RoomUnavailableException>(
            () => schedule.Reserve(2, booker, Period(11, 13)));
    }

    [Fact]
    public void GivenCancelledBooking_WhenConfirming_ShouldRejectAndKeepTimeFree()
    {
        var schedule = new RoomSchedule(new Room(1, "Meeting room"));
        var booking = schedule.Reserve(1, booker, Period(10, 12));
        schedule.Cancel(booking.Id);

        Should.Throw<InvalidOperationException>(() => schedule.Confirm(booking.Id));

        booking.Status.ShouldBe(BookingStatus.Cancelled);
        schedule.Reserve(2, booker, Period(10, 12)).Status.ShouldBe(BookingStatus.Reserved);
    }

    [Fact]
    public void GivenUnknownBooking_WhenConfirming_ShouldRejectAndLeaveReservationUnchanged()
    {
        var schedule = new RoomSchedule(new Room(1, "Meeting room"));
        var booking = schedule.Reserve(1, booker, Period(10, 12));

        Should.Throw<KeyNotFoundException>(() => schedule.Confirm(2));

        booking.Status.ShouldBe(BookingStatus.Reserved);
    }

    [Fact]
    public void GivenConfirmedBooking_WhenConfirmingAgain_ShouldRemainConfirmed()
    {
        var schedule = new RoomSchedule(new Room(1, "Meeting room"));
        var booking = schedule.Reserve(1, booker, Period(10, 12));
        schedule.Confirm(booking.Id);

        schedule.Confirm(booking.Id);

        booking.Status.ShouldBe(BookingStatus.Confirmed);
    }

    private static TimePeriod Period(int startHour, int endHour) => new(
        new DateTime(2026, 11, 1, startHour, 0, 0),
        new DateTime(2026, 11, 1, endHour, 0, 0));
}
