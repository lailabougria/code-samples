using AzureFunctions.Messages;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

public class SomethingHappenedHandler : IHandleMessages<ISomethingFailedEvent>
{
    static readonly ILog Log = LogManager.GetLogger<SomethingHappenedHandler>();
    public Task Handle(ISomethingFailedEvent message, IMessageHandlerContext context)
    {
        Log.Info("Oh no, something happened and a message went to the error queue:" + message.Whathappened);
        return Task.CompletedTask;
    }
}