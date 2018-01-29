//using System.Data.SQLite;

using System;
using System.Threading.Tasks;
using TaxiObjects;

namespace TaxiLoadingData
{
    public class LoadTaxis
    {
        public async Task<TaxiAutorization> TaxiFromPlateSqlite(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
                return null;

            
            plateNumber = plateNumber.Trim().Replace(" ", "").Replace("-","");

            if (plateNumber.Length < 3)
                return null;

            if (plateNumber.Substring(0, 1).ToUpper() == "B")
            {
                var buc = new LoadBucarestTaxis();
                return await buc.TaxiFromPlateSqlite(plateNumber);
            }
            if (plateNumber.Substring(0, 2).ToUpper() == "CJ")
            {
                var cj = new LoadClujTaxis();
                return await cj.TaxiFromPlateSqlite(plateNumber);

            }
            //maybe search in both?
            return null;
        }
        public async Task<TaxiAutorizations> GetTaxis(string city)
        {
            switch (city)
            {
                case "Bucarest":
                    var buc = new LoadBucarestTaxis();
                    return await buc.TaxiFromPlateSqliteAll();
                case "Cluj":
                    var cluj = new LoadClujTaxis();
                    return await cluj.TaxiFromPlateSqliteAll();
                default:
                    throw new ArgumentException("only for " + string.Join(",", GetCities()));
            }
        }
        public string[] GetCities()
        {
            return new string[] { "Bucarest", "Cluj" };
        }

    }
}
