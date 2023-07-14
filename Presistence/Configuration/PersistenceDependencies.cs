using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presistence.Contexts;

namespace Presistence.Configuration
{
    public static class PersistenceDependencies
    {
        /// <summary>
        /// Add Db Connections
        /// </summary>
        /// <param name="services">IServiceCollection to Extend</param>
        /// <param name="configuration">IConfiguration to access jwt settings</param>
        /// <param name="connectionStringName">Connection string name</param>
        /// <returns>Extended IServiceCollection</returns>
        public static IServiceCollection AddPersistence(this IServiceCollection services,
                                                        IConfiguration configuration,
                                                        string connectionStringName)
        {
            services.AddDbContext<ApplicationDbContext>(
                        options =>
                        {
                            options.UseSqlServer(configuration.GetConnectionString(connectionStringName),
                                    sqlServerOptionsAction: sqlOptions =>
                                    {
                                        sqlOptions.EnableRetryOnFailure(
                                            maxRetryCount: 10,
                                            maxRetryDelay: TimeSpan.FromSeconds(30),
                                            errorNumbersToAdd: null);
                                    });
                        });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
