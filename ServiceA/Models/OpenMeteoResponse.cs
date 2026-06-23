using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ServiceA.Models
{
    public class OpenMeteoResponse
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public CurrentWeather? Current { get; set; }
        public class CurrentWeather
        {
            [JsonPropertyName("time")]
            public string? Time { get; set; }

            [JsonPropertyName("temperature_2m")]
            public double Temperature2m { get; set; }

            [JsonPropertyName("relative_humidity_2m")]
            public int RelativeHumidity2m { get; set; }

            [JsonPropertyName("weather_code")]
            public int WeatherCode { get; set; }
        }
    }
}
