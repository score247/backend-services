namespace Soccer.EventProcessors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Caching.Memory;
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using GreenPipes;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Sentry;
    using Soccer.Database;
    using Soccer.EventProcessors.Matches;

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
            RegisterCache(services);
            RegisterLogging(services);
            RegisterDatabase(services);
            RegisterRabbitMq(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });

            Dictionary<string, ConnectionConfiguration> connections = Configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .ToDictionary(connection => connection.Key,
                              connection => new ConnectionConfiguration(connection.Key, connection.Value));

            DbSettingProviderManager
                .StartNewSession()
                .Use(connections)
                // It comes with a default connection string provider, which works well with MySql connections,
                // as well as a default DbSetting provider
                .WithMySql(resourcePath: Configuration["AppDataPath"])
                .Run();

            SqlMappers.RegisterJsonTypeHandlers();
        }

        private void RegisterCache(IServiceCollection services)
        {
            services.AddMemoryCache(Configuration.GetSection("fanex.caching"));
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
            services.AddScoped<FetchPreMatchesConsumer>();
            services.AddScoped<UpdatePostMatchesConsumer>();

            var bus = Bus.Factory.CreateUsingRabbitMq(
                   cfg =>
                   {
                       var host = cfg.Host(Configuration.GetSection("MessageQueue:RabbitMQ:Host").Value, "/", h =>
                       {
                           h.Username(Configuration.GetSection("MessageQueue:RabbitMQ:UserName").Value);
                           h.Password(Configuration.GetSection("MessageQueue:RabbitMQ:Password").Value);
                       });

                       cfg.ReceiveEndpoint(host, "score247", e =>
                       {
                           e.PrefetchCount = 16;
                           e.UseMessageRetry(x =>
                           {
                               x.Interval(2, 100);
                               x.Handle<Exception>((ex) =>
                               {
                                   var logger = services.BuildServiceProvider().GetRequiredService<ILogger>();
                                   logger.Error(ex.Message, ex);

                                   return true;
                               });
                           });
                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<FetchPreMatchesConsumer>());
                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<UpdatePostMatchesConsumer>());
                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<UpdateLiveMatchesConsumer>());
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