namespace Soccer.EventProcessors.Shared.Middlewares
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Soccer.EventProcessors.Shared.Configurations;

    public static class AppSettingsMiddleware
    {
        public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = new AppSettings(configuration);
            services.AddSingleton<IAppSettings>(appSettings);
        }
    }
}
