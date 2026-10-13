using System.Collections.Concurrent;

namespace TE.DomainStorytellingApplied.Domain;

public class BookingSystem
{
    private readonly ConcurrentDictionary<int, RoomSchedule> schedules = new();
    private int nextBookingId;
    private readonly IPaymentGateway payments;
    private readonly NotificationService notifications;
    private readonly TimeProvider clock;
    private readonly TimeSpan reservationDuration;

    public BookingSystem(IPaymentGateway payments, NotificationService notifications,
        TimeProvider? clock = null, TimeSpan? reservationDuration = null)
    {
        ArgumentNullException.ThrowIfNull(payments);
        ArgumentNullException.ThrowIfNull(notifications);

        this.payments = payments;
        this.notifications = notifications;
        this.clock = clock ?? TimeProvider.System;
        this.reservationDuration = reservationDuration ?? TimeSpan.FromMinutes(10);
        if (this.reservationDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(reservationDuration));
        }
    }

    public ReservationResult Reserve(Booker booker, Room room, TimePeriod period)
    {
        ArgumentNullException.ThrowIfNull(booker);
        ArgumentNullException.ThrowIfNull(room);
        ArgumentNullException.ThrowIfNull(period);

        var schedule = schedules.GetOrAdd(room.Id, _ => new RoomSchedule(room, clock, reservationDuration));
        int bookingId = Interlocked.Increment(ref nextBookingId);
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

    public void ExpireReservations()
    {
        foreach (var schedule in schedules.Values)
        {
            schedule.ExpireReservations();
        }
    }

    private RoomSchedule FindSchedule(int roomId) =>
        schedules.TryGetValue(roomId, out var schedule)
            ? schedule
            : throw new KeyNotFoundException("Rummets schema finns inte.");
}
