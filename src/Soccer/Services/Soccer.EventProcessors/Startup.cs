namespace Soccer.EventProcessors
{
    using System;
    using Fanex.Caching;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Soccer.EventProcessors.Shared.Middlewares;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<Func<DateTime>>(() => DateTime.Now);
            services.AddRabbitMq(Configuration);
            services.AddHealthCheck();
            services.AddLogging();
            services.AddDatabase();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            app.UseCors(options => options.AllowAnyOrigin());
            app.UseHealthCheck();
            app.UseRabbitMq();
            app.ConfigureExceptionHandler();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });

            app.UseCors(options => options.AllowAnyOrigin());

            app.UseHealthCheck();

            Dictionary<string, ConnectionConfiguration> connections = Configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .ToDictionary(connection => connection.Key,
                              connection => new ConnectionConfiguration(connection.Key, connection.Value));

            DbSettingProviderManager
                .StartNewSession()
                .Use(connections)
                .WithMySql(resourcePath: Configuration["AppDataPath"])
                .Run();

            SqlMappers.RegisterJsonTypeHandlers();

            var bus = app.ApplicationServices.GetService<IBusControl>();

            bus.Start();
        }

        private static void RegisterCache(IServiceCollection services)
        {
            services.AddSingleton<ICacheService, CacheService>();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<Func<DateTime>>(() => DateTime.Now);
        }

        private void RegisterLogging(IServiceCollection services)
        {
            LogManager
                 .SetDefaultLogCategory(Configuration["Fanex.Logging:DefaultCategory"])
                 .Use(new SentryLogging(new SentryEngineOptions
                 {
                     Dsn = new Dsn(Configuration["Fanex.Logging:SentryUrl"])
                 }));

            services.AddSingleton(Logger.Log);
        }

        private static void RegisterDatabase(IServiceCollection services)
            => services.AddSingleton<IDynamicRepository, DynamicRepository>();

        private void RegisterRabbitMq(IServiceCollection services)
        {
            var messageQueueSettings = new MessageQueueSettings();
            Configuration.Bind("MessageQueue", messageQueueSettings);

            services.AddMassTransit(serviceCollectionConfigurator =>
            {
                serviceCollectionConfigurator.AddConsumer<FetchPreMatchesConsumer>();
                serviceCollectionConfigurator.AddConsumer<FetchPostMatchesConsumer>();
                serviceCollectionConfigurator.AddConsumer<CloseLiveMatchConsumer>();
                serviceCollectionConfigurator.AddConsumer<ReceiveMatchEndEventConsumer>();
                serviceCollectionConfigurator.AddConsumer<ReceiveNormalEventConsumer>();
                serviceCollectionConfigurator.AddConsumer<ReceivePenaltyEventConsumer>();
                serviceCollectionConfigurator.AddConsumer<ProcessMatchEventConsumer>();
                serviceCollectionConfigurator.AddConsumer<OddsChangeConsumer>();
                serviceCollectionConfigurator.AddConsumer<UpdateMatchConditionsConsumer>();
            });

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
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

                    e.Consumer<FetchPreMatchesConsumer>(provider);
                    e.Consumer<FetchPostMatchesConsumer>(provider);
                    e.Consumer<CloseLiveMatchConsumer>(provider);
                    e.Consumer<UpdateMatchConditionsConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents", e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<ReceiveMatchEndEventConsumer>(provider);
                    e.Consumer<ReceiveNormalEventConsumer>(provider);
                    e.Consumer<ProcessMatchEventConsumer>(provider);
                });

                cfg.ReceiveEndpoint(host, $"{messageQueueSettings.QueueName}_MatchEvents_Penalties", e =>
                {
                    e.PrefetchCount = 1;
                    e.UseMessageRetry(RetryAndLogError(services));

                    e.Consumer<ReceivePenaltyEventConsumer>(provider);
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

        private static Action<GreenPipes.Configurators.IRetryConfigurator> RetryAndLogError(IServiceCollection services)
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