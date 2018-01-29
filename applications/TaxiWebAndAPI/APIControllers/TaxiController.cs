using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            LoadTaxis lt = new LoadTaxis();
            return  lt.GetCities();
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
            LoadTaxis lt = new LoadTaxis();
            return await lt.GetTaxis(city);
        }
        /// <summary>
        /// Get the taxy from the number - or null ( 204) if not found
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<TaxiAutorization> GetTaxi(string plateNumber)
        {
            
            var all = new LoadTaxis();
            return await all.TaxiFromPlateSqlite(plateNumber);
        }
        [HttpPost]
        public async Task<TaxiAutorization> GetFromPicture(string base64Picture)
        {
            var taxi = await GetNumberFromPicture(base64Picture);
            if (string.IsNullOrWhiteSpace(taxi))
                return null;
            return await GetTaxi(taxi);
        }
        [HttpPost]
        public async Task<string> GetNumberFromPicture(string base64Picture)
        {
            //return base64Picture == null ? "null" : "ASD"+base64Picture.Length;
            string url = "https://testocr1s.azurewebsites.net/api/MyOCR?code=B1pNAijXy4GLAysR3Tv3EkPmX9uhJl5Td4lYLfaSbpDp4o9xu1oMLw==";

            using (var httpClient = new HttpClient())
            {

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var cs = new StringContent("{\"name\": \"" + base64Picture + "\" }", Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(url, cs);

                response.EnsureSuccessStatusCode();
                var taxiLicense = await response.Content.ReadAsStringAsync();
                taxiLicense = taxiLicense.Replace("\"", "");
                return taxiLicense;
            }
        }
    }
}
