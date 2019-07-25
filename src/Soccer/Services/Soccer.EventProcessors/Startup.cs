namespace Soccer.EventProcessors
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.Repository;
    using GreenPipes;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Soccer.Core;
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
                .WithMySql(resourcePath: Configuration["AppDataPath"]) // It comes with a default connection string provider, which works well with MySql connections, as well as a default DbSetting provider
                .Run();

            SqlMappers.RegisterJsonTypeHandlers();
        }

        private void RegisterDatabase(IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
        }

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
                           e.UseMessageRetry(x => x.Interval(2, 100));
                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<FetchPreMatchesConsumer>());
                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<UpdatePostMatchesConsumer>());
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