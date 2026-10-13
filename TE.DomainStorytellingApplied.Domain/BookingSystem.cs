namespace TE.DomainStorytellingApplied.Domain;

public class BookingSystem(IPaymentGateway payments, NotificationService notifications)
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

    public PaymentAttempt Pay(int roomId, int bookingId)
    {
        var schedule = FindSchedule(roomId);
        var attempt = schedule.StartPayment(bookingId);
        bool succeeded = payments.Pay(attempt.Id);
        RecordPaymentResult(roomId, bookingId, attempt.Id, succeeded);
        return attempt;
    }

    public void RecordPaymentResult(int roomId, int bookingId, Guid paymentAttemptId, bool succeeded)
    {
        var schedule = FindSchedule(roomId);
        bool newlyConfirmed = schedule.RecordPaymentResult(bookingId, paymentAttemptId, succeeded);
        if (newlyConfirmed)
        {
            var booking = schedule.FindBooking(bookingId);
            notifications.Send(booking.Booker.Email,
                $"Din bokning av {booking.Room.Name} är bekräftad: " +
                $"{booking.Period.Start:yyyy-MM-dd HH:mm}–{booking.Period.End:yyyy-MM-dd HH:mm}.");
        }
    }

    public void Cancel(int roomId, int bookingId) => FindSchedule(roomId).Cancel(bookingId);

    private RoomSchedule FindSchedule(int roomId) =>
        schedules.TryGetValue(roomId, out var schedule)
            ? schedule
            : throw new KeyNotFoundException("Rummets schema finns inte.");
}
