using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class RoomScheduleTests
{
    private readonly Booker booker = new(1, "Anna", "anna@example.com");
 // kontrollerar att överlappande bokningar nekas
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

        existing.IsConfirmed.ShouldBeTrue();
    }
    //bokningar utan överlappning bekräftas
    [Theory]
    [InlineData(8, 9)]
    [InlineData(9, 10)]
    [InlineData(12, 13)]
    [InlineData(13, 14)]
    public void GivenNonOverlappingBooking_WhenReserving_ShouldConfirm(int startHour, int endHour)
    {
        var schedule = new RoomSchedule(new Room(1, "Meeting room"));
        schedule.Reserve(1, booker, Period(10, 12));

        var booking = schedule.Reserve(2, booker, Period(startHour, endHour));

        booking.IsConfirmed.ShouldBeTrue();
    }
    // kontrollerar att avbokning fungerar
    [Fact]
    public void GivenCancelledBooking_WhenReservingSameTime_ShouldConfirm()
    {
        var schedule = new RoomSchedule(new Room(1, "Meeting room"));
        var first = schedule.Reserve(1, booker, Period(10, 12));
        schedule.Cancel(first.Id);

        var second = schedule.Reserve(2, booker, Period(10, 12));

        first.IsConfirmed.ShouldBeFalse();
        second.IsConfirmed.ShouldBeTrue();
        Should.Throw<RoomUnavailableException>(
            () => schedule.Reserve(3, booker, Period(10, 12)));
    }
    // kontrollerar att dubbla boknings id nekas
    [Fact]
    public void GivenDuplicateId_WhenReservingFreeTime_ShouldReject()
    {
        var schedule = new RoomSchedule(new Room(1, "Meeting room"));
        schedule.Reserve(1, booker, Period(10, 12));

        Should.Throw<ArgumentException>(() => schedule.Reserve(1, booker, Period(12, 13)));

        schedule.Reserve(2, booker, Period(12, 13)).IsConfirmed.ShouldBeTrue();
    }
    // kontrollerar att avbokning av okänd bokning inte ändrar andra bokningar
    [Fact]
    public void GivenUnknownBooking_WhenCancelling_ShouldNotFreeExistingTime()
    {
        var schedule = new RoomSchedule(new Room(1, "Meeting room"));
        schedule.Reserve(1, booker, Period(10, 12));

        Should.Throw<KeyNotFoundException>(() => schedule.Cancel(2));
        Should.Throw<RoomUnavailableException>(() => schedule.Reserve(2, booker, Period(10, 12)));
    }

    private static TimePeriod Period(int startHour, int endHour) => new(
        new DateTime(2026, 11, 1, startHour, 0, 0),
        new DateTime(2026, 11, 1, endHour, 0, 0));
}
