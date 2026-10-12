namespace TE.DomainStorytellingApplied.Domain;

// Aggregatrot för ett rums bokningar. Använd samma schema för samma rum.
public class RoomSchedule(Room room)
{
    private readonly List<Booking> bookings = [];

    public int Id => Room.Id;
    public Room Room { get; } = room;
    // kollar om bokningsid redan finns i schemat innan en ny bokning skapas.
    public Booking Reserve(int bookingId, Booker booker, TimePeriod period)
    {
        if (bookings.Any(existing => existing.Id == bookingId))
        {
            throw new ArgumentException("Boknings-id finns redan i schemat.", nameof(bookingId));
        }
        //kollar om bokningen överlappar med befintliga bokningar
        bool overlaps = bookings.Any(existing =>
            existing.IsConfirmed &&
            period.Start < existing.Period.End &&
            period.End > existing.Period.Start);

        if (overlaps)
        {
            throw new RoomUnavailableException();
        }

        var booking = new Booking(bookingId, Room, period, booker);
        // Bekräfta bokningen innan den läggs till i schemat
        booking.Confirm();
        bookings.Add(booking);
        return booking;
    }
    // Avbokar en befintlig bokning
    public void Cancel(int bookingId)
    {
        var booking = bookings.SingleOrDefault(existing => existing.Id == bookingId)
            ?? throw new KeyNotFoundException("Bokningen finns inte i rummets schema.");

        booking.Cancel();
    }
}
