using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Infrastructure.Persistence;
using MassTransit;
using DebtCollector.Infrastructure.Services;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Azure.Core;

namespace DebtCollector.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DebtCollectorContext");
            
            services.AddDbContext<DebtCollectorContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    builder => builder.MigrationsAssembly(typeof(DebtCollectorContext).Assembly.FullName));
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<DebtCollectorContext>());

            services.AddScoped<AuthorizationService>();
            services.AddScoped<ITokenService>(provider => provider.GetRequiredService<AuthorizationService>());
            services.AddScoped<ICurrentUserService>(provider => provider.GetRequiredService<AuthorizationService>());

            services.AddScoped<IEmailService, AzureEmailService>();
            services.AddSingleton<IBlobStorageService>(provider => new BlobStorageService(configuration.GetConnectionString("AzureBlobStorage") ?? throw new InvalidOperationException("Connection string 'AzureBlobStorage' not found.")));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
