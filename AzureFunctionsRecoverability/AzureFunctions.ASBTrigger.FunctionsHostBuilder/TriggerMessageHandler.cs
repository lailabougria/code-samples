using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using System;

#region TriggerMessageHandler

public class TriggerMessageHandler : IHandleMessages<TriggerMessage>
{
    static readonly ILog Log = LogManager.GetLogger<TriggerMessageHandler>();
    readonly CustomComponent customComponent;

    public TriggerMessageHandler(CustomComponent customComponent)
    {
        this.customComponent = customComponent;
    }

    public Task Handle(TriggerMessage message, IMessageHandlerContext context)
    {
        Log.Warn($"Handling {nameof(TriggerMessage)} in {nameof(TriggerMessageHandler)}");
        Log.Warn($"Custom component returned: {customComponent.GetValue()}");

        throw new Exception("boom");

        //return context.SendLocal(new FollowupMessage());
    }
}

#endregion