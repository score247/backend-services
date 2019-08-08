namespace Soccer.EventPublishers.Shared.Middlewares
{
    using System;
    using Fanex.Logging;
    using GreenPipes;
    using GreenPipes.Configurators;
    using MassTransit;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Soccer.Core._Shared.Configurations;
    using Soccer.EventPublishers.Matches;
    using Soccer.EventPublishers.Odds;

    public static class RabbitMqMiddleware
    {
        private const int prefetCount = 16;

        public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var messageQueueSettings = new MessageQueueSettings();
            configuration.Bind("MessageQueue", messageQueueSettings);
            services.AddTransient<ProcessMatchEventPublisher>();
            services.AddTransient<OddsChangePublisher>();

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

                      cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents", e =>
                      {
                          e.PrefetchCount = prefetCount;
                          e.UseMessageRetry(RetryAndLogError(services));

                          e.Consumer<ProcessMatchEventPublisher>(serviceProvider);
                      });

                      cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_OddsEvents", e =>
                      {
                          e.PrefetchCount = prefetCount;
                          e.UseMessageRetry(RetryAndLogError(services));

                          e.Consumer<OddsChangePublisher>(serviceProvider);
                      });
                  }));
            });
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