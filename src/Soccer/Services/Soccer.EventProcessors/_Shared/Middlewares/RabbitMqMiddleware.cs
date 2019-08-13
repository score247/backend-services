namespace Soccer.EventProcessors.Shared.Middlewares
{
    using System;
    using Fanex.Logging;
    using GreenPipes;
    using GreenPipes.Configurators;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Soccer.Core.Shared.Configurations;
    using Soccer.EventProcessors.Matches;
    using Soccer.EventProcessors.Matches.MatchEvents;
    using Soccer.EventProcessors.Odds;

    public static class RabbitMqMiddleware
    {
        public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<FetchPreMatchesConsumer>();
            services.AddScoped<FetchPostMatchesConsumer>();
            services.AddScoped<CloseLiveMatchConsumer>();
            services.AddScoped<ReceiveMatchEventConsumer>();
            services.AddScoped<ProcessMatchEventConsumer>();
            services.AddScoped<PenaltyEventConsumer>();
            services.AddScoped<MatchEndEventConsumer>();
            services.AddScoped<RedCardEventConsumer>();
            services.AddScoped<OddsChangeConsumer>();
            services.AddScoped<UpdateMatchConditionsConsumer>();

            var messageQueueSettings = new MessageQueueSettings();
            configuration.Bind("MessageQueue", messageQueueSettings);

            services.AddMassTransit(s =>
            {
                s.AddBus(serviceProvider => Bus.Factory.CreateUsingRabbitMq(
                   cfg =>
                   {
                       var host = cfg.Host(
                           messageQueueSettings.Host,
                           messageQueueSettings.VirtualHost, h =>
                           {
                               h.Username(messageQueueSettings.Username);
                               h.Password(messageQueueSettings.Password);
                           });

                       cfg.ReceiveEndpoint(host, messageQueueSettings.QueueName, e =>
                       {
                           e.PrefetchCount = 16;
                           e.UseMessageRetry(RetryAndLogError(services));

                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<FetchPreMatchesConsumer>());
                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<FetchPostMatchesConsumer>());
                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<CloseLiveMatchConsumer>());
                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<UpdateMatchConditionsConsumer>());
                       });

                       cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents", e =>
                       {
                           e.PrefetchCount = 16;
                           e.UseMessageRetry(RetryAndLogError(services));

                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<ProcessMatchEventConsumer>());
                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<ReceiveMatchEventConsumer>());
                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<MatchEndEventConsumer>());
                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<PenaltyEventConsumer>());
                           e.Consumer<RedCardEventConsumer>(serviceProvider);
                       });

                       cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_Odds", e =>
                       {
                           e.PrefetchCount = 16;
                           e.UseMessageRetry(RetryAndLogError(services));

                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<OddsChangeConsumer>());
                       });
                   }));
            });
        }

        public static void UseRabbitMq(this IApplicationBuilder application)
        {
            var bus = application.ApplicationServices.GetService<IBusControl>();

            bus.Start();
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