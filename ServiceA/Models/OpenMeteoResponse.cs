using System.Text.Json.Serialization;

namespace ServiceA.Models;

public class OpenMeteoResponse
{
    public double Latitude { get; set; }

    public double Longitude { get; set; }
    public CurrentWeather? Current { get; set; }

    public class CurrentWeather
    {
        [JsonPropertyName("time")] public string? Time { get; set; }

        [JsonPropertyName("temperature_2m")] public double Temperature2M { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public int RelativeHumidity2M { get; set; }

        [JsonPropertyName("weather_code")] public int WeatherCode { get; set; }
    }
}