namespace Soccer.EventProcessors.Shared.Middlewares
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
    using Soccer.EventProcessors.Leagues;
    using Soccer.EventProcessors.Matches;
    using Soccer.EventProcessors.Matches.MatchEvents;
    using Soccer.EventProcessors.News;
    using Soccer.EventProcessors.Teams;
    using Soccer.EventProcessors.Timeline;

    public static class RabbitMqMiddleware
    {
        private const int PrefetchCount = 16;
        private const int retryInterval = 100;

#pragma warning disable S138 // Functions should not have too many lines of code

        public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
#pragma warning restore S138 // Functions should not have too many lines of code
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
                serviceCollectionConfigurator.AddConsumer<PeriodStartEventConsumer>();
                serviceCollectionConfigurator.AddConsumer<BreakStartEventConsumer>();
                serviceCollectionConfigurator.AddConsumer<ReceiveMatchEventConsumer>();
                serviceCollectionConfigurator.AddConsumer<ProcessMatchEventConsumer>();
                serviceCollectionConfigurator.AddConsumer<UpdateMatchConditionsConsumer>();
                serviceCollectionConfigurator.AddConsumer<UpdateTeamStatisticConsumer>();
                serviceCollectionConfigurator.AddConsumer<UpdateMatchCoverageConsumer>();
                serviceCollectionConfigurator.AddConsumer<FetchedLiveMatchConsumer>();
                serviceCollectionConfigurator.AddConsumer<FetchLeaguesConsumer>();
                serviceCollectionConfigurator.AddConsumer<FetchTimelinesConsumer>();
                serviceCollectionConfigurator.AddConsumer<FetchCommentaryConsumer>();
                serviceCollectionConfigurator.AddConsumer<FetchMatchLineupsConsumer>();
                serviceCollectionConfigurator.AddConsumer<FetchHeadToHeadConsumer>();
                serviceCollectionConfigurator.AddConsumer<CleanMajorLeaguesCacheConsumer>();
                serviceCollectionConfigurator.AddConsumer<FetchLeaguesSeasonConsumer>();
                serviceCollectionConfigurator.AddConsumer<FetchLeagueStandingConsumer>();
                serviceCollectionConfigurator.AddConsumer<FetchedLeagueMatchesConsumer>();
                serviceCollectionConfigurator.AddConsumer<RemoveTimelinesConsumer>();
                serviceCollectionConfigurator.AddConsumer<FetchedLeagueGroupConsumer>();
                serviceCollectionConfigurator.AddConsumer<FetchNewsConsumer>();
            });

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(
                       messageQueueSettings.Host,
                       port: messageQueueSettings.Port,
                       messageQueueSettings.VirtualHost, h =>
                       {
                           h.Username(messageQueueSettings.Username);
                           h.Password(messageQueueSettings.Password);
                           h.Heartbeat(300);
                       });

                cfg.ReceiveEndpoint(host, messageQueueSettings.QueueName, e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<FetchPreMatchesConsumer>(provider);
                    e.Consumer<FetchPostMatchesConsumer>(provider);
                    e.Consumer<CloseLiveMatchConsumer>(provider);
                    e.Consumer<UpdateMatchConditionsConsumer>(provider);
                    e.Consumer<UpdateMatchCoverageConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<ReceiveMatchEventConsumer>(provider);
                    e.Consumer<ProcessMatchEventConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents_PeriodStart", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<PeriodStartEventConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents_BreakStart", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<BreakStartEventConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents_MatchEnd", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<MatchEndEventConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents_RedCard", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<RedCardEventConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents_Penalties", e =>
                {
                    e.PrefetchCount = 1;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<PenaltyEventConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_LiveMatches", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<FetchedLiveMatchConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_TeamStatistic", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<UpdateTeamStatisticConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_Timelines", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<FetchTimelinesConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_Commentaries", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<FetchCommentaryConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_Leagues", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<FetchLeaguesConsumer>(provider);
                    e.Consumer<FetchLeaguesSeasonConsumer>(provider);
                    e.Consumer<FetchedLeagueMatchesConsumer>(provider);
                    e.Consumer<FetchedLeagueGroupConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_HeadToHead", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<FetchHeadToHeadConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchLineups", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<FetchMatchLineupsConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_CleanMajorLeaguesCache", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<CleanMajorLeaguesCacheConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_League_Standing", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<FetchLeagueStandingConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_ConfirmedTimelines", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<RemoveTimelinesConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_News", e =>
                {
                    e.PrefetchCount = PrefetchCount;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<FetchNewsConsumer>(provider);
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
            => x =>
            {
                x.Interval(1, retryInterval);
                x.Handle<Exception>((ex) =>
                {
                    var logger = services.BuildServiceProvider().GetRequiredService<ILogger>();
                    logger.Error(ex.Message, ex);

                    return true;
                });
            };
    }
}