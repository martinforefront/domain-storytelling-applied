namespace TE.DomainStorytellingApplied.Domain;


public class RoomSchedule(Room room)
{
    private readonly List<Booking> bookings = [];

    public int Id => Room.Id;
    public Room Room { get; } = room;

    public Booking Reserve(int bookingId, Booker booker, TimePeriod period)
    {
        if (bookings.Any(existing => existing.Id == bookingId))
        {
            throw new ArgumentException("Boknings-id finns redan i schemat.", nameof(bookingId));
        }

        bool overlaps = bookings.Any(existing =>
            existing.IsConfirmed &&
            period.Start < existing.Period.End &&
            period.End > existing.Period.Start);

        if (overlaps)
        {
            throw new RoomUnavailableException();
        }

        var booking = new Booking(bookingId, Room, period, booker);

        booking.Confirm();
        bookings.Add(booking);
        return booking;
    }

    public void Cancel(int bookingId)
    {
        var booking = bookings.SingleOrDefault(existing => existing.Id == bookingId)
            ?? throw new KeyNotFoundException("Bokningen finns inte i rummets schema.");

        booking.Cancel();
    }
}
