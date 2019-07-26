namespace Soccer.API._Shared.Middlewares
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Soccer.API._Shared.Configurations;

    public static class AppSettingsMiddleware
    {
        public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = new AppSettings(configuration);
            services.AddSingleton<IAppSettings>(appSettings);
        }
    }
}