using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Soccer.API.Shared.Configurations;

namespace Soccer.API.Shared.Middlewares
{
    public static class AuthenticationMiddleware
    {
        public static void AddAuthentication(this IServiceCollection services, IAppSettings appSettings)
        {
            if (appSettings.EnabledAuthentication)
            {
                var key = Encoding.ASCII.GetBytes(appSettings.JwtSecretKey);
                IdentityModelEventSource.ShowPII = true;
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            }
        }

        public static void ConfigureAuthentication(this IApplicationBuilder application)
        {
            var appSettings = application.ApplicationServices.GetService<IAppSettings>();
            if (appSettings?.EnabledAuthentication == true)
            {
                application.UseAuthentication();
                application.UseAuthorization();
            }
        }
    }
}