using Core.Interfaces.Shared.Services;
using Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MOCA.Services.Implementation.Shared;
using Services.Implementation.Shared;
using System.Reflection;

namespace Services.Configuration
{
    public static class ServicesDependencies
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services,
                                                         IConfiguration configuration)
        {
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
            services.AddScoped<IPhoneNumberService, PhoneNumberService>();
            services.AddScoped<IUploadImageService, UploadImageService>();
            services.AddScoped<IUploadFileService, UploadFileService>();

            services.Configure<FileSettings>(configuration.GetSection("FileSettings"));

            return services;
        }

        public static IServiceCollection AddMapper(this IServiceCollection services,
                                                   Assembly[] assemblies)
        {
            services.AddAutoMapper(assemblies);
            return services;
        }
    }
}
