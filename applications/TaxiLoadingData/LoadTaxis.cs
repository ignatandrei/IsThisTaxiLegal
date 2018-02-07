//using System.Data.SQLite;

using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (plateNumber.Substring(0, 2).ToUpper() == "NT") // this might be an issue; how about Piatra Neamt;
            {
                var tm = new LoadRoman();
                return await tm.TaxiFromPlateSqlite(plateNumber);

            }
            //maybe search in both?
            return null;
        }
        public async Task<TaxiAutorizations> GetTaxis(string city)
        {
            switch (city.ToLower())
            {
                case "bucuresti":
                    var buc = new LoadBucarestTaxis();
                    return await buc.TaxiFromPlateSqliteAll();
                case "cluj":
                    var cluj = new LoadClujTaxis();
                    return await cluj.TaxiFromPlateSqliteAll();
                case "timisoara":
                    var tm = new LoadTimisoaraTaxis();
                    return await tm.TaxiFromPlateSqliteAll();
                default:
                    throw new ArgumentException("only for " + string.Join(",", GetCities()));
            }
        }
        public async Task<string> GetRandomTaxiNumber()
        {
            try
            {

                string city = null;

                using (var con = new SqliteConnection())
                {
                    con.ConnectionString = "Data Source=taxis.sqlite3;";


                    await con.OpenAsync();
                    while (city == null)
                    {
                        var id = DateTime.Now.Ticks % 3 + 1;
                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "select [City] from [LatestData] where id=" + id;

                            using (var rd = await cmd.ExecuteReaderAsync())
                            {
                                while (await rd.ReadAsync())
                                {
                                    city = rd["City"].ToString();
                                }
                            }
                        }
                    }

                }
                var all = (await GetTaxis(city)).Where(it => it.State == LicenceState.Valid).ToArray();
                var max = all.Length;
                var rnd = new Random(DateTime.Now.Millisecond);
                var taxiAut = all[rnd.Next(0, max - 1)].CarLicensed.PlateNumber;
                return taxiAut;
            }
            catch (Exception ex)
            {

                throw;
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