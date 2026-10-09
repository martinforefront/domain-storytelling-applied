namespace TE.DomainStorytellingApplied.Domain;

// Chosen time (Value object).
public record TimePeriod
{
    public DateTime Start { get; }
    public DateTime End { get; }
// Validation for the time period.
    public TimePeriod(DateTime start, DateTime end)
    {
        if (end <= start)
        {
            throw new ArgumentException("End must be later than start.", nameof(end));
        }

        Start = start;
        End = end;
    }
}
