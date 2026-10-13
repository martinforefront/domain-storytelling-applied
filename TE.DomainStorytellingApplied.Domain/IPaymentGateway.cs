namespace TE.DomainStorytellingApplied.Domain;

public interface IPaymentGateway
{
    // The attempt ID identifies the payment at the provider too.
    // False means an explicit decline; an exception means the outcome is unknown.
    bool Pay(Guid paymentAttemptId);
}
