using System.Text.Json;

namespace MrJB.MS.Common.Helpers
{
    public static class JsonSerializerHelper
    {
        public static JsonSerializerOptions Message = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

        public static JsonSerializerOptions HumanReadable = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
    }
}
