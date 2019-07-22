namespace Soccer.EventProcessors
{
    using GreenPipes;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Soccer.Core.Domain;
    using Soccer.Core.Domain.Matches;
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
            services.AddScoped<IMatchRepository, MatchRepository>();
            services.AddScoped<FetchPreMatchConsumer>();

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
        }

        private void RegisterDatabase(IServiceCollection services)
        {
            var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();

            services.AddEntityFrameworkNpgsql()
                   .AddDbContext<SoccerContext>(options
                        => options
                            .UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
                            .EnableSensitiveDataLogging()
                            .UseLoggerFactory(loggerFactory))
                   .BuildServiceProvider();
        }

        private void RegisterRabbitMq(IServiceCollection services)
        {
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
                           e.Consumer(() => services.BuildServiceProvider().GetRequiredService<FetchPreMatchConsumer>());
                       });
                   });

            bus.Start();

            services.AddMassTransit(s =>
            {
                s.AddConsumer<FetchPreMatchConsumer>();
                s.AddBus(_ => bus);
            });
        }
    }
}