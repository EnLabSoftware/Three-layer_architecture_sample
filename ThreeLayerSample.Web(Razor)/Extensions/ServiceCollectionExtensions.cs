using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThreeLayerSample.Domain.Interfaces;
using ThreeLayerSample.Domain.Interfaces.Services;
using ThreeLayerSample.Domain.Models;
using ThreeLayerSample.Infrastructure;
using ThreeLayerSample.Service;

namespace ThreeLayerSample.Web_Razor_.Extensions
{
	public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add needed instances for database
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            // Configure DbContext with Scoped lifetime   
            services.AddDbContext<DemoContext>(options =>
                {
                    options.UseSqlServer(AppSettings.ConnectionString,
                        sqlOptions => sqlOptions.CommandTimeout(120));
                    options.UseLazyLoadingProxies();
                }
            );

            services.AddScoped<Func<DemoContext>>((provider) => () => provider.GetService<DemoContext>());
            services.AddScoped<DbFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        /// <summary>
        /// Add instances of in-use services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services.AddScoped<IWorkService, WorkService>();
        }
    }
}
