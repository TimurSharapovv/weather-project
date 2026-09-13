namespace ServiceA.Models;

public class WeatherDto
{
   public float Temperature{get; init;}
   public float Humidity{get; init;}
   public string? Description{get; init;}
   public string? Time{get; init;}
}