﻿using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;

namespace WeatherData.Services
{
    public interface IAddDataService
    {
        Task<bool> ConvertAndAdd(IBrowserFile browserFile);
    }
}