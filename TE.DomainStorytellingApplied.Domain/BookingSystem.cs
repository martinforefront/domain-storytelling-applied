namespace TE.DomainStorytellingApplied.Domain;

public class BookingSystem
{
    private readonly Dictionary<int, RoomSchedule> schedules = [];
    private int nextBookingId = 1;

    public ReservationResult Reserve(Booker booker, Room room, TimePeriod period)
    {
        if (!schedules.TryGetValue(room.Id, out var schedule))
        {
            schedule = new RoomSchedule(room);
            schedules.Add(room.Id, schedule);
        }

        int bookingId = nextBookingId++;
        try
        {
            var booking = schedule.Reserve(bookingId, booker, period);
            return new ReservationResult(booking);
        }
        catch (RoomUnavailableException)
        {
            return new ReservationResult(null);
        }
    }

    public void Cancel(int roomId, int bookingId)
    {
        if (!schedules.TryGetValue(roomId, out var schedule))
        {
            throw new KeyNotFoundException("Rummets schema finns inte.");
        }

        schedule.Cancel(bookingId);
    }
}
