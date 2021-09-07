using AzureFunctions.Messages;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Transport;
using System;
using System.Threading.Tasks;

#region configuration-with-function-host-builder

[assembly: FunctionsStartup(typeof(Startup))]
[assembly: NServiceBusTriggerFunction(Startup.EndpointName)]

public class Startup : FunctionsStartup
{
    public const string EndpointName = "ASBTriggerQueue";

    public override void Configure(IFunctionsHostBuilder builder)
    {
        var services = builder.Services;

        // register custom service in the container
        services.AddSingleton(_ =>
        {
            var configurationRoot = builder.GetContext().Configuration;
            var customComponentInitializationValue = configurationRoot.GetValue<string>("CustomComponentValue");

            return new CustomComponent(customComponentInitializationValue);
        });

        builder.UseNServiceBus(() =>
        {
            var endpointConfig = new ServiceBusTriggeredEndpointConfiguration(EndpointName);
            endpointConfig.AdvancedConfiguration.EnableFeature<GrabMessageSessionFeature>();
            endpointConfig.AdvancedConfiguration.Recoverability().Delayed(settings => settings.NumberOfRetries(MaxImmediateRetries));
            endpointConfig.AdvancedConfiguration.Recoverability().Immediate(settings => settings.NumberOfRetries(MaxDelayedRetries));
            // endpointConfig.AdvancedConfiguration.Recoverability().Failed(settings => settings.OnMessageSentToErrorQueue(failedMsg =>
            // {
            //     GrabMessageSessionFeature.Session?.Publish<ISomethingFailedEvent>(ev => ev.Whathappened = failedMsg.Exception.Message);
            //     return Task.CompletedTask;
            // }));
            endpointConfig.AdvancedConfiguration.Recoverability().CustomPolicy(RecoverabilityPolicy);
            return endpointConfig;
        });
    }

    RecoverabilityAction RecoverabilityPolicy(RecoverabilityConfig config, ErrorContext context)
    {
        if (context.ImmediateProcessingFailures == MaxImmediateRetries && context.DelayedDeliveriesPerformed == MaxDelayedRetries) // retries depleted
        {
            GrabMessageSessionFeature.Session?.Publish<ISomethingFailedEvent>(ev => ev.Whathappened = context.Exception.Message);
            return RecoverabilityAction.Discard("Retries depleted");
        }

        if (context.ImmediateProcessingFailures < MaxImmediateRetries)
            return RecoverabilityAction.ImmediateRetry();
        
        return RecoverabilityAction.DelayedRetry(TimeSpan.FromSeconds(DelayTime));
    }

    const double DelayTime = 1;
    const int MaxDelayedRetries = 1;
    const int MaxImmediateRetries = 2;
}

#endregion