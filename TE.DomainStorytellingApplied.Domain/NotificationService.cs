namespace TE.DomainStorytellingApplied.Domain;


public class NotificationService(TextWriter output)
{
    public void Send(string recipient, string message)
    {
        output.WriteLine($"Till: {recipient}");
        output.WriteLine(message);
    }
}
