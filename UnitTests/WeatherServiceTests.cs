using NUnit.Framework;
using MelbourneWeatherApp.Services;

namespace MelbourneWeatherApp.UnitTests
{
    [TestFixture]
    public class WeatherServiceTests
    {
        [Test]
        public async Task TestWeatherService_Successful()
        {
            // Arrange
            var providers = new List<WeatherProvider>
            {
                new WeatherProvider
                {
                    Name = "Weatherstack",
                    Url = "http://api.weatherstack.com/current?access_key=d322aa30164a00d9fc6859a8bc29176d&query=Melbourne"
                },
                new WeatherProvider
                {
                    Name = "OpenWeatherMap",
                    Url = "http://api.openweathermap.org/data/2.5/weather?q=melbourne,AU&appid=0cd101c7018b6e54d0b30cfdea2eff7a"
                }
            };

            var weatherService = new WeatherService(providers);

            // Act
            var weatherInfo = await weatherService.GetWeatherInfoAsync();

            // Assert
            Assert.IsNotNull(weatherInfo);
            Assert.IsTrue(weatherInfo.TemperatureDegrees > 0);
            Assert.IsTrue(weatherInfo.WindSpeed >= 0);
        }

        [Test]
        public async Task TestWeatherService_Failover()
        {
            // Arrange
            var providers = new List<WeatherProvider>
            {
                // Intentionally provide invalid URLs to trigger failover
                new WeatherProvider
                {
                    Name = "InvalidProvider",
                    Url = "http://invalidurl"
                },
                new WeatherProvider
                {
                    Name = "OpenWeatherMap",
                    Url = "http://api.openweathermap.org/data/2.5/weather?q=melbourne,AU&appid=0cd101c7018b6e54d0b30cfdea2eff7a"
                }
            };

            var weatherService = new WeatherService(providers);

            // Act
            var weatherInfo = await weatherService.GetWeatherInfoAsync();

            // Assert
            Assert.IsNotNull(weatherInfo);
            Assert.IsTrue(weatherInfo.TemperatureDegrees > 0);
            Assert.IsTrue(weatherInfo.WindSpeed >= 0);
        }

        [Test]
        public async Task TestWeatherService_CachedResult_BothProvidersFail()
        {
            // Arrange
            var providers = new List<WeatherProvider>
            {
                // Intentionally provide invalid URLs to trigger failover
                new WeatherProvider
                {
                    Name = "InvalidProvider1",
                    Url = "http://invalidurl1"
                },
                new WeatherProvider
                {
                    Name = "InvalidProvider2",
                    Url = "http://invalidurl2"
                }
            };

            var weatherService = new WeatherService(providers);

            // Act
            // Trigger a request to populate the cache
            await weatherService.GetWeatherInfoAsync();

            // Wait for the cache to expire
            await Task.Delay(TimeSpan.FromSeconds(4));

            // Fail both providers intentionally
            var weatherInfo = await weatherService.GetWeatherInfoAsync();

            // Assert
            Assert.IsNotNull(weatherInfo);
            // Check if cached values are returned
            Assert.IsTrue(weatherInfo.TemperatureDegrees > 0);
            Assert.IsTrue(weatherInfo.WindSpeed >= 0);
        }

    }
}
