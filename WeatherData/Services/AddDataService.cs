using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WeatherData.Data;
using WeatherData.Entities;
using System.Globalization;

namespace WeatherData.Services
{
    public class AddDataService : IAddDataService
    {
        private readonly WeatherDataContext _weatherDataContext;

        public AddDataService(WeatherDataContext weatherDataContext)
        {
            _weatherDataContext = weatherDataContext;
        }


        /// <summary>
        /// Denna metoden tar in en fil som användaren har valt,
        /// filen läses in som en temporär fil på hårddisken och tas bort så fort den inte behövs mer.
        /// Använder OpenReadStream(20000000) då filen var större än default värdet.
        /// När datan har lästs in från fil, skapar vi upp entiteter som läggs i en lista
        /// som vi sedan skickar in till databasen och sparar.
        /// </summary>
        /// <param name="browserFile">Filen som användaren har valt.</param>
        /// <returns>En boolean på om det lyckats eller inte.</returns>
        public async Task<bool> ConvertAndAdd(IBrowserFile browserFile)
        {
            var dataList = new List<TemperatureReading>();
            var filePath = Path.GetTempFileName();

            using (var stream = File.Create(filePath))
            {
                await browserFile.OpenReadStream(20000000).CopyToAsync(stream);
            }

            var data = await File.ReadAllLinesAsync(filePath);
            File.Delete(filePath);

            for (int i = 0; i < data.Length; i++)
            {
                var item = data[i];
                if (item != null)
                {
                    var splittedData = item.Split(",");

                    if (splittedData.Length == 4)
                    {
                        try
                        {
                            TemperatureReading temperatureReading = new TemperatureReading()
                            {
                                Date = DateTime.Parse(splittedData[0].Replace(" ", "T")),
                                Thermometer = splittedData[1],
                                Temperature = double.Parse(splittedData[2], NumberFormatInfo.InvariantInfo),
                                Humidity = int.Parse(splittedData[3])
                            };
                            dataList.Add(temperatureReading);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            System.Diagnostics.Debug.WriteLine(i + 1);
                            return false;
                        }
                    }
                }
            }
            await _weatherDataContext.temperatureReadings.AddRangeAsync(dataList);
            await _weatherDataContext.SaveChangesAsync();
            return true;
        }
    }
}
