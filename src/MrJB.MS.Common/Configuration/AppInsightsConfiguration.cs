namespace MrJB.MS.Common.Configuration;
    
public class AppInsightsConfiguration
{
    public const string Position = "ApplicationInsights";

    public string RoleName { get; set; }

    public string InstrumentationKey { get; set; }
}
