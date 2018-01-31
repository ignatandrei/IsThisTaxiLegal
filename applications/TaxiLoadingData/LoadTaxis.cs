//using System.Data.SQLite;

using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
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


            plateNumber = plateNumber.Trim().Replace(" ", "").Replace("-", "");

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
            if (plateNumber.Substring(0, 2).ToUpper() == "TM")
            {
                var tm = new LoadTimisoaraTaxis();
                return await tm.TaxiFromPlateSqlite(plateNumber);

            }
            //maybe search in both?
            return null;
        }
        public async Task<TaxiAutorizations> GetTaxis(string city)
        {
            switch (city)
            {
                case "Bucuresti":
                    var buc = new LoadBucarestTaxis();
                    return await buc.TaxiFromPlateSqliteAll();
                case "Cluj":
                    var cluj = new LoadClujTaxis();
                    return await cluj.TaxiFromPlateSqliteAll();
                case "Timisoara":

                default:
                    throw new ArgumentException("only for " + string.Join(",", GetCities()));
            }
        }
        public async Task<string[]> GetCities()
        {
            //return new string[] { "Bucuresti", "Cluj","Timisoara" };
            List<string> data = new List<string>();
            using (var con = new SqliteConnection())
            {
                con.ConnectionString = "Data Source=taxis.sqlite3;";

                //con.ConnectionString = "taxis.sqlite3";
                await con.OpenAsync();

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select [City] from [LatestData]";

                    using (var rd = await cmd.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            data.Add(rd["City"].ToString());
                        }
                    }
                }

            }
            return data.ToArray();
        }
    }
}