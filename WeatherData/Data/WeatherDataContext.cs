using Microsoft.EntityFrameworkCore;
using WeatherData.Entities;

namespace WeatherData.Data
{
    public class WeatherDataContext : DbContext
    {
        public WeatherDataContext(DbContextOptions options) : base(options) { }

        public DbSet<TemperatureReading> temperatureReadings { get; set; }

    }
}
