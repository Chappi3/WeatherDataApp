using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherData.Entities;
using WeatherData.Models;

namespace WeatherData.Services
{
    public interface IFetchDataService
    {
        double FetchAverageTempOnDateChanged(string calendarDate, string thermometer);
        Task<List<TemperatureReadingModel>> FetchData();
    }
}