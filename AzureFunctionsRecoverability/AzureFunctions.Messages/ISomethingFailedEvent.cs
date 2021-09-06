namespace AzureFunctions.Messages
{
    using NServiceBus;

    public interface ISomethingFailedEvent : IEvent
    {
        string Whathappened { get; set; }
    }
}