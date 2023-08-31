using Microsoft.AspNetCore.Mvc;
using MelbourneWeatherApp.Services;
using MelbourneWeatherApp.Models;

namespace MelbourneWeatherApp.Controllers
{
    public class WeatherController : Controller
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("v1/weather")]
        public async Task<IActionResult> GetWeatherAsync()
        {
            var weatherInfo = await _weatherService.GetWeatherInfoAsync();
            return Ok(new WeatherInfo
            {
                WindSpeed = weatherInfo.WindSpeed,
                TemperatureDegrees = weatherInfo.TemperatureDegrees
            });
        }
    }
}
