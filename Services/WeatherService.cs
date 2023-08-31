using Newtonsoft.Json;
using MelbourneWeatherApp.Models;

namespace MelbourneWeatherApp.Services
{
    public class WeatherService
    {
        private readonly List<WeatherProvider> _providers;
        private readonly HttpClient _httpClient;
        private WeatherInfo _cachedWeatherInfo;
        private DateTime _cacheExpiration;
        private int _currentProviderIndex;

        public WeatherService(List<WeatherProvider> providers)
        {
            _providers = providers;
            _httpClient = new HttpClient();
            _cachedWeatherInfo = new WeatherInfo();
            _cacheExpiration = DateTime.MinValue;
            _currentProviderIndex = 0;
        }

        public async Task<WeatherInfo> GetWeatherInfoAsync()
        {
            if (DateTime.UtcNow > _cacheExpiration)
            {
                for (int i = 0; i < _providers.Count; i++)
                {
                    int providerIndex = (_currentProviderIndex + i) % _providers.Count;
                    var provider = _providers[providerIndex];

                    try
                    {
                        var response = await _httpClient.GetAsync(provider.Url);
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var weatherInfo = ParseWeatherInfo(provider.Name, content);
                            if (weatherInfo != null)
                            {
                                _cachedWeatherInfo = weatherInfo;
                                _cacheExpiration = DateTime.UtcNow.AddSeconds(3);
                                _currentProviderIndex = providerIndex;
                                break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Ignore exceptions and try the next provider
                    }
                }
            }

            return _cachedWeatherInfo;
        }

        private WeatherInfo ParseWeatherInfo(string providerName, string content)
        {
            try
            {
                WeatherInfo weatherInfo = new WeatherInfo();

                if (providerName == "Weatherstack")
                {
                    dynamic weatherStackResponse = JsonConvert.DeserializeObject<dynamic>(content);
                    weatherInfo.WindSpeed = (double)weatherStackResponse.current.wind_speed;
                    weatherInfo.TemperatureDegrees = (double)weatherStackResponse.current.temperature;
                }
                else if (providerName == "OpenWeatherMap")
                {
                    dynamic openWeatherMapResponse = JsonConvert.DeserializeObject<dynamic>(content);
                    weatherInfo.WindSpeed = (double)openWeatherMapResponse.wind.speed;
                    // Convert temperature from Kelvin to Celsius
                    weatherInfo.TemperatureDegrees = (double)openWeatherMapResponse.main.temp - 273.15;
                }
                else
                {
                    return null; // Unsupported provider
                }

                return weatherInfo;
            }
            catch (Exception)
            {
                return null; // Parsing failed
            }
        }

    }
}
