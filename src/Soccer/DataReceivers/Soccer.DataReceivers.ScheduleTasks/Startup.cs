namespace Soccer.DataReceivers.ScheduleTasks
{
    using System;
    using System.Linq;
    using Hangfire;
    using Hangfire.Dashboard;
    using Hangfire.PostgreSql;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        private const int OneYearDays = 365;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(x => x.UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireConnection"),
                   new PostgreSqlStorageOptions
                   {
                       //change this
                       InvisibilityTimeout = TimeSpan.FromDays(OneYearDays)
                   }));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = Enumerable.Empty<IDashboardAuthorizationFilter>()
            });

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}