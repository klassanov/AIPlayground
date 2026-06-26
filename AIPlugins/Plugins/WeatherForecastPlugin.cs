using System.ComponentModel;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.SemanticKernel;

namespace AIPlugins.Plugins
{

    internal class WeatherForecastPlugin
    {
        private static string ForecastJson = @"{
  ""latitude"": 42.6875,
  ""longitude"": 23.3125,
  ""generationtime_ms"": 0.147342681884766,
  ""utc_offset_seconds"": 10800,
  ""timezone"": ""Europe/Sofia"",
  ""timezone_abbreviation"": ""GMT+3"",
  ""elevation"": 555,
  ""current_units"": {
    ""time"": ""iso8601"",
    ""interval"": ""seconds"",
    ""is_day"": """",
    ""apparent_temperature"": ""°C"",
    ""rain"": ""mm"",
    ""showers"": ""mm"",
    ""weather_code"": ""wmo code""
  },
  ""current"": {
    ""time"": ""2026-06-26T16:15"",
    ""interval"": 900,
    ""is_day"": 1,
    ""apparent_temperature"": 29.4,
    ""rain"": 0,
    ""showers"": 0,
    ""weather_code"": 3
  },
  ""daily_units"": {
    ""time"": ""iso8601"",
    ""weather_code"": ""wmo code""
  },
  ""daily"": {
    ""time"": [
      ""2026-06-26"",
      ""2026-06-27"",
      ""2026-06-28"",
      ""2026-06-29"",
      ""2026-06-30"",
      ""2026-07-01"",
      ""2026-07-02""
    ],
    ""weather_code"": [3, 3, 2, 0, 3, 80, 80]
  }
}";
        private HttpClient httpClient;

        public WeatherForecastPlugin(IHttpClientFactory httpClientFactory)
        {
            this.httpClient = httpClientFactory.CreateClient("WeatherForecast");
        }


        [KernelFunction("get_weather_forecast")]
        [Description("Get the weather forecast for a given location, specified by latitude and longitude.")]
        [return: Description("Returns a JSON string containing the weather forecast data.")]
        public async Task<string> GetWeatherForecast(double latitude, double longitude)
        {
            var queryStringParameters = new Dictionary<string, string>()
            {
                { "latitude", latitude.ToString() },
                { "longitude", longitude.ToString() },
                { "daily", "weather_code" },
                { "timezone", "auto" },
                { "current", "is_day,apparent_temperature,rain,showers,weather_code" },
            };

            var fullUri = QueryHelpers.AddQueryString(httpClient.BaseAddress!.ToString(), queryStringParameters!);

            var result = await httpClient.GetAsync(fullUri);
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            return json;
        }
    }
}

