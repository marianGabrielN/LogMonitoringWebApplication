using LogMonitoring.Application.Interfaces;
using LogMonitoring.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LogMonitoring.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ILogParser, LogParser>();
        return services;
    }
}
