using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxiLoadingData;
using TaxiObjects;

namespace TaxiWebAndAPI.APIControllers
{
    public class TaxiController:Controller
    {
        public string LoadBucarest()
        {
            try
            {
                var buc = new LoadBucarestTaxis();
                var data = buc.TaxisFromCSV().Item1;
            }
            catch (Exception ex)
            {

                return ex.StackTrace;
            }

            return "ok";
        }
        public string[] GetCities()
        {
            return new string[] { "Bucarest" };
        }
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
        public TaxiAutorization GetTaxi(string plateNumber)
        {
            return Program.BucarestTaxis.FirstOrDefault(it => it.CarLicensed.PlateNumber.ToLower() == plateNumber?.ToLower());
        }
    }
}
