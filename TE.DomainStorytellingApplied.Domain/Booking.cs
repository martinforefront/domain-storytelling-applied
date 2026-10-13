namespace TE.DomainStorytellingApplied.Domain;

public enum BookingStatus
{
    Reserved,
    Confirmed,
    Cancelled
}

public class Booking
{
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
    internal void Cancel() => Status = BookingStatus.Cancelled;
}
