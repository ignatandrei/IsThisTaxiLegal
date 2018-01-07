using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxiLoadingData;
using TaxiObjects;

namespace TaxiWebAndAPI.APIControllers
{
    [Route("API/[controller]/[action]")]//for swagger
    public class TaxiController:Controller
    {
        //public string LoadBucarest()
        //{
        //    try
        //    {
        //        var buc = new LoadBucarestTaxis();
        //        var data = buc.TaxisFromCSV().Item1;
        //    }
        //    catch (Exception ex)
        //    {

        //        return ex.StackTrace;
        //    }

        //    return "ok";
        //}
        /// <summary>
        /// Get all cities registered
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string[] GetCities()
        {
            return new string[] { "Bucarest" };
        }
        /// <summary>
        /// Get all taxis for a city
        /// <see cref="GetCities"/>
        /// </summary>
        /// <param name="city">the city name</param>
        /// <returns>an array of <see cref="TaxiAutorization"/></returns>
        [HttpGet]
        public TaxiAutorizations GetTaxis(string city)
        {
            switch (city)
            {
                case "Bucarest":
                    return Program.BucarestTaxis;
                default:
                    throw new ArgumentException("only for " + string.Join(",", GetCities()));
            }
        }
        /// <summary>
        /// Get the taxy from the number - or null ( 204) if not found
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public TaxiAutorization GetTaxi(string plateNumber)
        {
            return Program.BucarestTaxis.FirstOrDefault(it => it.CarLicensed.PlateNumber.ToLower() == plateNumber?.ToLower());
        }
    }
}
