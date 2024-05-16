using HealthChecks.UI.Client;
using HealthChecks.UI.Configuration;
using HealthChecksAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Health Checks
builder.Services.ConfigureHealthChecks(builder.Configuration);
builder.Services.AddHealthChecksUI(opt =>
{
    opt.SetEvaluationTimeInSeconds(10); // time in seconds between check    
    opt.MaximumHistoryEntriesPerEndpoint(60); // maximum history of checks    
    opt.SetApiMaxActiveRequests(1); // api requests concurrency    
    opt.AddHealthCheckEndpoint("feedback api", "/api/health"); // map health check api    
})
.AddInMemoryStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// Health Check Middleware
app.MapHealthChecks("/api/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.UseHealthChecksUI(options =>
{
    options.UIPath = "/healthcheck-ui";
   // options.AddCustomStylesheet("./HealthCheck/Custom.css");
});

app.Run();
