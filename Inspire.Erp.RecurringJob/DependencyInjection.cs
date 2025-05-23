using Hangfire;
using Inspire.Erp.Infrastructure;
using Inspire.Erp.RecurringJob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddRecurringJob(this IServiceCollection services, IConfiguration config)
    {
        var settings = config.GetSection("ApplicationSettings").Get<ApplicationSettings>();

        services.AddHangfire(configuration =>
            configuration.UseSqlServerStorage(settings.ConnectionString));

        services.AddHangfireServer();

        services.AddScoped<IEmailSchedulerService, EmailSchedulerService>();


        return services;
    }
}
