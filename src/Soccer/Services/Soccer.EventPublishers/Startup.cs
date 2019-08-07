namespace Soccer.Services.EventPublishers
{
    using System;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using GreenPipes;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Sentry;
    using Soccer.EventPublishers.Matches;
    using Soccer.EventPublishers.Matches.Hubs;

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
            RegisterLogging(services);
            RegisterRabbitMq(services);
            services.AddSignalR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSignalR(routes =>
            {
                routes.MapHub<MatchEventHub>("/hubs/MatchEventHub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });
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

        private void RegisterRabbitMq(IServiceCollection services)
        {
            services.AddScoped<ProcessMatchEventPublisher>();

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

                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<ProcessMatchEventPublisher>());
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