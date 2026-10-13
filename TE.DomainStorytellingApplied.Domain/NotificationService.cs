namespace TE.DomainStorytellingApplied.Domain;


public class NotificationService(TextWriter output)
{
    private readonly object gate = new();

    public void Send(string recipient, string message)
    {
        lock (gate)
        {
            output.WriteLine($"Till: {recipient}");
            output.WriteLine(message);
        }
    }
}
