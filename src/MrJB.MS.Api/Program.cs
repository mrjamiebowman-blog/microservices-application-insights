using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MrJB.MS.Common.ApplicationInsights.Filters;
using MrJB.MS.Common.Configuration;
using MrJB.MS.Common.Extensions;
using MrJB.MS.Common.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// app insights
var options = new ApplicationInsightsServiceOptions();
options.EnableHeartbeat = true;
options.InstrumentationKey = builder.Configuration.GetValue<string>($"{AppInsightsConfiguration.Position}:{nameof(AppInsightsConfiguration.InstrumentationKey)}");
builder.Services.AddApplicationInsightsTelemetry(options);

// app insights: filters
builder.Services
            .AddApplicationInsightsTelemetryProcessor<TelemetryFilterHealthChecks>()
            .AddApplicationInsightsTelemetryProcessor<TelemetryApiFilter>();

// services
builder.Services
    .AddCustomApplicationInsightsApi(builder.Configuration)
    .AddAzureServiceBusProducerConfiguration(builder.Configuration);

// health checks
builder.Services.AddCustomHealthChecks(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/hc/startup", new HealthCheckOptions()
{
    Predicate = (check) => check.Tags.Contains(HealthCheckExtensions.Tags.Startup),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/hc/ready", new HealthCheckOptions()
{
    Predicate = (check) => check.Tags.Contains(HealthCheckExtensions.Tags.Readiness),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/hc/live", new HealthCheckOptions()
{
    Predicate = (check) => check.Tags.Contains(HealthCheckExtensions.Tags.Liveness),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
