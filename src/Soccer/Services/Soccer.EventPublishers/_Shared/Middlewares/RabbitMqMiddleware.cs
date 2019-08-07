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

    public static class RabbitMqMiddleware
    {
        public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var messageQueueSettings = new MessageQueueSettings();
            configuration.Bind("MessageQueue", messageQueueSettings);
            services.AddTransient<ProcessMatchEventPublisher>();

            services.AddMassTransit(s =>
            {
                s.AddBus(_ => Bus.Factory.CreateUsingRabbitMq(
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
                          e.PrefetchCount = 16;
                          e.UseMessageRetry(RetryAndLogError(services));

                          e.Consumer<ProcessMatchEventPublisher>(services.BuildServiceProvider());
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