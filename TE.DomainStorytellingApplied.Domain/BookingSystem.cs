namespace TE.DomainStorytellingApplied.Domain;

public class BookingSystem(NotificationService notifications)
{
    private readonly Dictionary<int, RoomSchedule> schedules = [];
    private int nextBookingId = 1;

    public Booking Book(ContactDetails contactDetails, Room room, TimePeriod period)
    {
        ArgumentNullException.ThrowIfNull(contactDetails);

        if (!schedules.TryGetValue(room.Id, out var schedule))
        {
            schedule = new RoomSchedule(room);
            schedules.Add(room.Id, schedule);
        }

        int bookingId = nextBookingId++;
        Booking booking;
        try
        {
            booking = schedule.Reserve(bookingId, contactDetails, period);
        }
        catch (RoomUnavailableException)
        {
            notifications.Send(contactDetails.Email,
                "Bokningen kunde inte genomföras. Rummet är redan bokat.");

            return new Booking(bookingId, schedule.Room, period, contactDetails);
        }

        schedule.Confirm(booking.Id);

        notifications.Send(booking.ContactDetails.Email,
            $"Din bokning av {booking.Room.Name} är bekräftad: " +
            $"{booking.Period.Start:yyyy-MM-dd HH:mm}–{booking.Period.End:yyyy-MM-dd HH:mm}.");
        return booking;
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
