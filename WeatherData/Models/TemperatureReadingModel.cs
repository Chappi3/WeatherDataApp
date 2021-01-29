using System;

namespace WeatherData.Models
{
    public class TemperatureReadingModel
    {
        public string Thermometer { get; set; }
        public DateTime Date { get; set; }
        public double AverageTemperature { get; set; }
        public int AverageHumidity { get; set; }
        public int MoldRisk { get; set; }
    }
}
