using FeedbackService.Api.HealthCheck;
using FeedbackService.Api;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecksAPI
{
    public static class HealthCheck
    {
        public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(configuration["ConnectionStrings:DefaultConnection"], healthQuery: "select 1", name: "SQL servere", failureStatus: HealthStatus.Healthy, tags: new[] { "Feedback", "Database" })
                .AddCheck<RemoteHealthCheck>("Remote endpoints Health Check", failureStatus: HealthStatus.Healthy)
                .AddCheck<MemoryHealthCheck>($"Feedback Service Memory Check", failureStatus: HealthStatus.Healthy, tags: new[] { "Feedback Service" })
                .AddUrlGroup(new Uri("https://localhost:44377/api/v1/heartbeats/ping"), name: "base URL", failureStatus: HealthStatus.Healthy);

            //services.AddHealthChecksUI();
            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(10); //time in seconds between check    
                opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
                opt.SetApiMaxActiveRequests(1); //api requests concurrency    
                opt.AddHealthCheckEndpoint("feedback api", "/api/health"); //map health check api    

            })
                .AddInMemoryStorage();
        }
    }
}
