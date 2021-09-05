using Core.Helpers.App;
using Infrastructure.Services.AppServices;
using Infrastructure.Services.ThirdPartyServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBackend.StartupExtensions.AppServicesAndConfig
{
    public static class AppAndThirdPartyServicesExt
    {

        public static IServiceCollection AddAppAndThirdPartyServices(this IServiceCollection services, IConfiguration config)
        {
            // App Services
            services.AddScoped<EmailService>();
            services.AddScoped<EmailSenderService>();
            services.AddScoped<SmsService>();
            services.AddScoped<SmsSenderService>();
            services.AddScoped<ImageCloudService>();

            //
            services.AddScoped<AppSettingsService>();

            // Add Mapping Tools
            services.AddAutoMapper(typeof(AppMappingProfiles));
            // to use CloudinarySettings with appsettings data in Cloudinary section
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));





            return services;
        }
    }
}
