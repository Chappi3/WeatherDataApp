using System;

namespace WeatherData.Entities
{
    public class TemperatureReading
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Thermometer { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
    }
}
