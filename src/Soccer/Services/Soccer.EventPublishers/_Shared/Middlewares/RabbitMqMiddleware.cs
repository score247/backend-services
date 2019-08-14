namespace Soccer.EventPublishers.Shared.Middlewares
{
    using System;
    using Fanex.Logging;
    using GreenPipes;
    using GreenPipes.Configurators;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Soccer.Core.Shared.Configurations;
    using Soccer.EventPublishers.Matches;
    using Soccer.EventPublishers.Odds;
    using Soccer.EventPublishers.Teams;

    public static class RabbitMqMiddleware
    {
        private const int prefetCount = 16;

        public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var messageQueueSettings = new MessageQueueSettings();
            configuration.Bind("MessageQueue", messageQueueSettings);

            services.AddMassTransit(serviceCollectionConfigurator =>
            {
                serviceCollectionConfigurator.AddConsumer<ProcessMatchEventPublisher>();
                serviceCollectionConfigurator.AddConsumer<UpdateTeamStatisticPublisher>();
                serviceCollectionConfigurator.AddConsumer<OddsMovementPublisher>();
                serviceCollectionConfigurator.AddConsumer<OddsComparisonPublisher>();
            });

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(
                    messageQueueSettings.Host,
                    messageQueueSettings.VirtualHost, h =>
                    {
                        h.Username(messageQueueSettings.Username);
                        h.Password(messageQueueSettings.Password);
                        h.Heartbeat(300);
                    });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents", e =>
                {
                    e.PrefetchCount = prefetCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<ProcessMatchEventPublisher>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_OddsMovement", e =>
                {
                    e.PrefetchCount = prefetCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<OddsComparisonPublisher>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_OddsComparison", e =>
                {
                    e.PrefetchCount = prefetCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<OddsMovementPublisher>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_Odds", e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<UpdateTeamStatisticPublisher>(provider);
                });
            }));

            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
        }

        public static void UseRabbitMq(this IApplicationBuilder application, IApplicationLifetime applicationLifetime)
        {
            var bus = application.ApplicationServices.GetService<IBusControl>();

            applicationLifetime.ApplicationStarted.Register(bus.Start);
            applicationLifetime.ApplicationStopped.Register(bus.Stop);
        }

#pragma warning disable S109 // Magic numbers should not be used

        private static Action<IRetryConfigurator> RetryAndLogError(IServiceCollection services)
        {
            return x =>
            {
                x.Interval(1, 100);
                x.Handle<Exception>((ex) =>
                {
                    var logger = services.BuildServiceProvider().GetRequiredService<ILogger>();
                    logger.Error(ex.Message, ex);

                    return true;
                });
            };
        }

#pragma warning restore S109 // Magic numbers should not be used
    }
}