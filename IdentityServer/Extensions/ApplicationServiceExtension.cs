using IdentityServer.DbContext;
using IdentityServer.Initializer;
using IdentityServer.Models;
using IdentityServer.Services;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using ServiceDiscovery;

namespace IdentityServer.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddIdentityServerServices(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddIdentityServer(config);
            services.AddServiceDiscovery(config);
            services.AddHealthChecks(config);
            return services;
        }

        // Health Check
        private static void AddHealthChecks(this IServiceCollection services, IConfiguration config)
        {
            services.AddHealthChecks()
                .AddSqlServer(config.GetSection("DatabaseSettings:ConnectionString").Value,
                    name: "Identity Server Database Health",
                    failureStatus: HealthStatus.Degraded);
        }
        
        // Service Discovery
        private static void AddServiceDiscovery(this IServiceCollection services, IConfiguration config)
        {
            ServiceConfig serviceConfig = config.GetServiceConfig();
            services.RegisterConsulServices(serviceConfig);
        }

        // Identity Server
        private static void AddIdentityServer(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(config.GetSection("DatabaseSettings:ConnectionString").Value);
            });
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddIdentityServer()
                .AddInMemoryClients(Config.Clients(config))
                .AddInMemoryApiScopes(Config.ApiScopes(config))
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential();

            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IProfileService, ProfileService>();
        }
    }
}