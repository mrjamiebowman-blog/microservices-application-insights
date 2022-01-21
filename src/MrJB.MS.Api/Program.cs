using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using MrJB.MS.Common.Configuration;
using MrJB.MS.Common.Extensions;
using MrJB.MS.Common.Services;

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

// services
builder.Services
    .AddTransient<IProducerService, ProducerAzureServiceBus>()
    .AddCustomApplicationInsightsApi(builder.Configuration)
    .AddAzureServiceBusProducerConfiguration(builder.Configuration);

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

app.Run();
