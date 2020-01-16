using System;
using System.Reflection;
using Fanex.Logging;
using MediatR;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Score247.Shared.Middlewares;
using Soccer.API._Shared.Formatters;
using Soccer.API.Shared.Middlewares;
using Soccer.Database;

namespace Soccer.API
{
    public class Startup
    {
        private bool EnableSwagger;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

#pragma warning disable S3902 // "Assembly.GetExecutingAssembly" should not be called

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddLogging(Configuration);
                var appSettings = services.AddSettings(Configuration);
                services.AddMediatR(Assembly.GetExecutingAssembly());
                services.AddServices();
                services.AddHealthCheck();
                EnableSwagger = appSettings.EnabledSwagger;
                if (EnableSwagger)
                {
                    services.AddSwagger();
                }
                services.AddMemoryCache();
                services.AddAuthentication(appSettings);
                services.AddCors(appSettings);
                services
                    .AddMvc()
                    .AddNewtonsoftJson()
                    .AddMvcOptions(option =>
                    {
                        option.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Instance));
                        option.InputFormatters.Add(new MessagePackInputFormatter(ContractlessStandardResolver.Instance));

                        if (appSettings.EnabledAuthentication)
                        {
                            var policy = new AuthorizationPolicyBuilder()
                                        .RequireAuthenticatedUser()
                                        .Build();
                            option.Filters.Add(new AuthorizeFilter(policy));
                        }
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
            }
        }

#pragma warning restore S3902 // "Assembly.GetExecutingAssembly" should not be called

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            try
            {
                app.AddCors();
                app.UseStaticFiles();
                app.ConfigureExceptionHandler();
                app.UseHealthCheck();
                app.UseDatabase(Configuration);
                if (EnableSwagger)
                {
                    app.ConfigureSwagger(Configuration);
                }
                app.UseRouting();

                app.ConfigureAuthentication();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                });
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
                SqlMappers.RegisterJsonTypeHandlers();

                app.UsePathBase(Configuration["HostingVirtualPath"]);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
            }

            LogService.Info("LiveScore API has been started.");
        }
    }
}