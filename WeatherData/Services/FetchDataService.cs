using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherData.Data;
using WeatherData.Entities;
using WeatherData.Models;
using Microsoft.EntityFrameworkCore;

namespace WeatherData.Services
{
    public class FetchDataService : IFetchDataService
    {
        private readonly WeatherDataContext _weatherDataContext;

        public FetchDataService(WeatherDataContext weatherDataContext)
        {
            _weatherDataContext = weatherDataContext;
        }


        /// <summary>
        /// Denna metoden hämtar alla temperaturer med valt datum och vald termometer.
        /// </summary>
        /// <param name="calendarDate">Det valda datumet</param>
        /// <param name="thermometer">Den valda termometern</param>
        /// <returns>Medelvärdet av temperaturen för valt datum.</returns>
        public double FetchAverageTempOnDateChanged(string calendarDate, string thermometer)
        {
            var date = DateTime.Parse(calendarDate);
            var tempsAtDate = _weatherDataContext.temperatureReadings
                .Where(t => (t.Date.Date == date.Date) && t.Thermometer == thermometer)
                .Select(t => t.Temperature)
                .ToList();
            return Math.Round(tempsAtDate.Sum() / tempsAtDate.Count, 1);
        }


        /// <summary>
        /// Denna metoden skapar en lista med alla de datum som finns i databasen.
        /// Sedan hämtar den all data för varje datum och skickar datan vidare för beräkning till
        /// <see cref="CalculateAndAddToList(List{TemperatureReading}, List{TemperatureReadingModel}, string, DateTime)"/>
        /// </summary>
        /// <returns>En lista med data byggda enligt modellen <see cref="TemperatureReadingModel"/></returns>
        public async Task<List<TemperatureReadingModel>> FetchData()
        {
            List<TemperatureReadingModel> readingModels = new List<TemperatureReadingModel>();

            var dates = _weatherDataContext.temperatureReadings
                .AsEnumerable()
                .GroupBy(t => t.Date.Date)
                .ToList();

            foreach (var date in dates)
            {
                var dateData = _weatherDataContext.temperatureReadings.Where(t => t.Date.Date == date.Key);
                CalculateAndAddToList(await dateData.Where(t => t.Thermometer == "Inne").ToListAsync(), readingModels, "Inne", date.Key);
                CalculateAndAddToList(await dateData.Where(t => t.Thermometer == "Ute").ToListAsync(), readingModels, "Ute", date.Key);
            }

            return readingModels;
        }


        /// <summary>
        /// Denna metoden räknar ut det som behövs och skapar upp en modell
        /// som sedan sparas i listan <paramref name="listToAdd"/>.
        /// </summary>
        /// <param name="listToCalc">Listan som ändvänds för att göra beräkningar med.</param>
        /// <param name="listToAdd">Listan som modellen ska hamna i.</param>
        /// <param name="thermometer">Vilken termometer som använts.</param>
        /// <param name="date">Datum som ska sparas i modellen.</param>
        private static void CalculateAndAddToList(
            List<TemperatureReading> listToCalc,
            List<TemperatureReadingModel> listToAdd,
            string thermometer,
            DateTime date)
        {
            double avgTemp = 0;
            int avgHumid = 0;

            foreach (var tempReading in listToCalc)
            {
                avgTemp += tempReading.Temperature;
                avgHumid += tempReading.Humidity;
            }

            avgTemp /= listToCalc.Count;
            avgHumid /= listToCalc.Count;

            var moldRisk = (avgHumid - 78) * (avgTemp / 15) / 0.22;
            if (moldRisk < 0)
            {
                moldRisk = 0;
            }
            else if (moldRisk > 100)
            {
                moldRisk = 100;
            }

            var readingModel = new TemperatureReadingModel()
            {
                Date = date,
                AverageTemperature = avgTemp,
                AverageHumidity = avgHumid,
                MoldRisk = (int)moldRisk,
                Thermometer = thermometer
            };

            listToAdd.Add(readingModel);
        }
    }
}
