using System;
using Soccer.Core.Shared.Configurations;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Soccer.DataReceivers.ScheduleTasks._Shared.Middlewares
{
    public static class RabbitMqMiddleware
    {
        public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var messageQueueSettings = new MessageQueueSettings();
            configuration.Bind("MessageQueue", messageQueueSettings);

            services.AddMassTransit(x =>
            {
                x.AddBus(_ => Bus.Factory.CreateUsingRabbitMq(
                    cfg =>
                    {
                        cfg.Host(
                            messageQueueSettings.Host,
                            messageQueueSettings.Port,
                            messageQueueSettings.VirtualHost, h =>
                            {
                                h.Username(messageQueueSettings.Username);
                                h.Password(messageQueueSettings.Password);
                            });
                    }));
            });
        }
    }
}