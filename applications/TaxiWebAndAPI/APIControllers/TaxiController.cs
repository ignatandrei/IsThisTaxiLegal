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
        [HttpGet]
        public KeyValuePair<string,int>[] GetLicenceStates()
        {
            return Enum.GetValues(typeof(LicenceState))
                .Cast<LicenceState>()
                .Select(it => new KeyValuePair<string, int>(it.ToString(), (int)it))
                .ToArray();
        }
        /// <summary>
        /// Get all taxis for a city
        /// <see cref="GetCities"/>
        /// </summary>
        /// <param name="city">the city name</param>
        /// <returns>an array of <see cref="TaxiAutorization"/></returns>
        [HttpGet]
        public async Task<TaxiAutorizations> GetTaxis(string city)
        {
            switch (city)
            {
                case "Bucarest":
                    var buc = new LoadBucarestTaxis();
                    return await buc.TaxiFromPlateSqliteAll();
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
        public async Task<TaxiAutorization> GetTaxi(string plateNumber)
        {
            var buc = new LoadBucarestTaxis();
            return await buc.TaxiFromPlateSqlite(plateNumber);
        }
    }
}
