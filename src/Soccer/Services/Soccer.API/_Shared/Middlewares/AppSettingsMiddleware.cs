namespace Soccer.API.Shared.Middlewares
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Soccer.API.Shared.Configurations;

    public static class AppSettingsMiddleware
    {
        public static IAppSettings AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = new AppSettings(configuration);
            services.AddSingleton<IAppSettings>(appSettings);

            return appSettings;
        }
    }
}