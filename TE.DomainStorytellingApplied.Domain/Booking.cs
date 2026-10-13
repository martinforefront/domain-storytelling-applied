namespace TE.DomainStorytellingApplied.Domain;

public enum BookingStatus
{
    Reserved,
    Confirmed,
    Cancelled
}

public class Booking
{
    private readonly List<PaymentAttempt> paymentAttempts = [];

    public IReadOnlyList<PaymentAttempt> PaymentAttempts => paymentAttempts.AsReadOnly();

    internal Booking(int id, Room room, TimePeriod period, Booker booker)
    {
        Id = id;
        Room = room;
        Period = period;
        Booker = booker;
    }

    public int Id { get; }
    public Room Room { get; }
    public TimePeriod Period { get; }
    public Booker Booker { get; }
    public BookingStatus Status { get; private set; } = BookingStatus.Reserved;
    public bool IsConfirmed => Status == BookingStatus.Confirmed;

    internal void Confirm()
    {
        if (Status == BookingStatus.Cancelled)
        {
            throw new InvalidOperationException("En avbruten bokning kan inte bekräftas.");
        }

        Status = BookingStatus.Confirmed;
    }
    internal PaymentAttempt StartPayment()
    {
        if (Status != BookingStatus.Reserved)
        {
            throw new InvalidOperationException("Bara en reserverad bokning kan betalas.");
        }

        if (paymentAttempts.Any(attempt => attempt.Status == PaymentStatus.Pending))
        {
            throw new InvalidOperationException("Ett betalningsförsök pågår redan.");
        }

        var attempt = new PaymentAttempt();
        paymentAttempts.Add(attempt);
        return attempt;
    }

    internal bool RecordPaymentResult(Guid paymentAttemptId, bool succeeded)
    {
        var attempt = paymentAttempts.SingleOrDefault(attempt => attempt.Id == paymentAttemptId)
            ?? throw new KeyNotFoundException("Betalningsförsöket finns inte på bokningen.");

        bool changed = attempt.RecordResult(succeeded);
        if (changed && succeeded)
        {
            Confirm();
            return true;
        }

        return false;
    }

    internal void Cancel()
    {
        if (paymentAttempts.Any(attempt => attempt.Status == PaymentStatus.Pending))
        {
            throw new InvalidOperationException("Invänta betalningsresultatet innan bokningen avbryts.");
        }

        Status = BookingStatus.Cancelled;
    }
}
