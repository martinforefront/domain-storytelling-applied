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
            (existing.Status is BookingStatus.Reserved or BookingStatus.Confirmed) &&
            period.Overlaps(existing.Period));

        if (overlaps)
        {
            throw new RoomUnavailableException();
        }

        var booking = new Booking(bookingId, Room, period, booker);

        bookings.Add(booking);
        return booking;
    }

    public PaymentAttempt StartPayment(int bookingId) => FindBooking(bookingId).StartPayment();

    public bool RecordPaymentResult(int bookingId, Guid paymentAttemptId, bool succeeded) =>
        FindBooking(bookingId).RecordPaymentResult(paymentAttemptId, succeeded);

    internal Booking FindBooking(int bookingId) =>
        bookings.SingleOrDefault(existing => existing.Id == bookingId)
        ?? throw new KeyNotFoundException("Bokningen finns inte i rummets schema.");

    public void Confirm(int bookingId)
    {
        var booking = FindBooking(bookingId);

        booking.Confirm();
    }

    public void Cancel(int bookingId)
    {
        var booking = FindBooking(bookingId);

        booking.Cancel();
    }
}
