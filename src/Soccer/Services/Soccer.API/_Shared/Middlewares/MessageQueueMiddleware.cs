namespace Soccer.API._Shared.Middlewares
{
    using GreenPipes;
    using MassTransit;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class MessageQueueMiddleware
    {
        public static void AddRabbitMq(IServiceCollection services, IConfiguration configuration)
        {
            // Register consumer here
            // Ex: services.AddScoped<FetchPreMatchesConsumer>();

            var bus = Bus.Factory.CreateUsingRabbitMq(
                   cfg =>
                   {
                       var host = cfg.Host(configuration.GetSection("MessageQueue:RabbitMQ:Host").Value, "/", h =>
                       {
                           h.Username(configuration.GetSection("MessageQueue:RabbitMQ:UserName").Value);
                           h.Password(configuration.GetSection("MessageQueue:RabbitMQ:Password").Value);
                       });

                       cfg.ReceiveEndpoint(host, "score247", e =>
                       {
                           e.PrefetchCount = 16;
                           e.UseMessageRetry(x => x.Interval(2, 100));

                           // Build consumer here
                           // Ex: e.Consumer(() => services.BuildServiceProvider().GetRequiredService<FetchPreMatchesConsumer>());
                       });
                   });

            bus.Start();

            services.AddMassTransit(s =>
            {
                s.AddBus(_ => bus);
            });
        }
    }
}