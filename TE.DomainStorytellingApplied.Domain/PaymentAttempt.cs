namespace TE.DomainStorytellingApplied.Domain;

public enum PaymentStatus
{
    Pending,
    Succeeded,
    Failed
}

public class PaymentAttempt
{
    internal PaymentAttempt()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;

    internal bool RecordResult(bool succeeded)
    {
        var result = succeeded ? PaymentStatus.Succeeded : PaymentStatus.Failed;
        if (Status == result)
        {
            return false;
        }

        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException("Betalningsförsöket har redan ett annat resultat.");
        }

        Status = result;
        return true;
    }
}
