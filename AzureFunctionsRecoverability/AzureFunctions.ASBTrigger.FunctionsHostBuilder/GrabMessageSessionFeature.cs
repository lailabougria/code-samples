using NServiceBus;
using NServiceBus.Features;
using System.Threading.Tasks;

public class GrabMessageSessionFeature : Feature
{
    public static IMessageSession Session { get; private set; }
    
    class GrabMessageSessionFeatureSetup : FeatureStartupTask
    {
        protected override Task OnStart(IMessageSession session)
        {
            Session = session;
            return Task.CompletedTask;
        }

        protected override Task OnStop(IMessageSession session)
        {
            Session = null;
            return Task.CompletedTask;
        }
    }

    protected override void Setup(FeatureConfigurationContext context)
    {
        context.RegisterStartupTask(new GrabMessageSessionFeatureSetup());
    }
}