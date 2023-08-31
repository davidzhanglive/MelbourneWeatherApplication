using MelbourneWeatherApp.Services;

namespace MelbourneWeatherApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new List<WeatherProvider>
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
            });
            services.AddSingleton<WeatherService>();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
