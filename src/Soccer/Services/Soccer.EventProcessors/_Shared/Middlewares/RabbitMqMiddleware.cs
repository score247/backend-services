﻿namespace Soccer.EventProcessors.Shared.Middlewares
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
    using Soccer.EventProcessors.Matches;
    using Soccer.EventProcessors.Matches.MatchEvents;
    using Soccer.EventProcessors.Odds;
    using Soccer.EventProcessors.Teams;

    public static class RabbitMqMiddleware
    {
        public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var messageQueueSettings = new MessageQueueSettings();
            configuration.Bind("MessageQueue", messageQueueSettings);

            services.AddMassTransit(serviceCollectionConfigurator =>
            {
                serviceCollectionConfigurator.AddConsumer<FetchPreMatchesConsumer>();
                serviceCollectionConfigurator.AddConsumer<FetchPostMatchesConsumer>();
                serviceCollectionConfigurator.AddConsumer<CloseLiveMatchConsumer>();
                serviceCollectionConfigurator.AddConsumer<MatchEndEventConsumer>();
                serviceCollectionConfigurator.AddConsumer<RedCardEventConsumer>();
                serviceCollectionConfigurator.AddConsumer<PenaltyEventConsumer>();
                serviceCollectionConfigurator.AddConsumer<ReceiveMatchEventConsumer>();
                serviceCollectionConfigurator.AddConsumer<ProcessMatchEventConsumer>();
                serviceCollectionConfigurator.AddConsumer<OddsChangeConsumer>();
                serviceCollectionConfigurator.AddConsumer<UpdateMatchConditionsConsumer>();
                serviceCollectionConfigurator.AddConsumer<UpdateTeamStatisticConsumer>();
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

                cfg.ReceiveEndpoint(host, messageQueueSettings.QueueName, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<FetchPreMatchesConsumer>(provider);
                    e.Consumer<FetchPostMatchesConsumer>(provider);
                    e.Consumer<CloseLiveMatchConsumer>(provider);
                    e.Consumer<UpdateMatchConditionsConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents", e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<ReceiveMatchEventConsumer>(provider);
                    e.Consumer<ProcessMatchEventConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents_MatchEnd", e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<MatchEndEventConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents_RedCard", e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<RedCardEventConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents_Penalties", e =>
                {
                    e.PrefetchCount = 1;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<PenaltyEventConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_TeamStatistic", e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<UpdateTeamStatisticConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_Odds", e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<OddsChangeConsumer>(provider);
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
    }
}