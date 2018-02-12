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

            TaxiAutorization ret = null;
            Dictionary<string, ILoadTaxis> all = new Dictionary<string, ILoadTaxis>();
            all.Add("BC", new LoadBacauTaxis());
            all.Add("B", new LoadBucarestTaxis());
            all.Add("CJ", new LoadClujTaxis());
            all.Add("NT", new LoadRoman());
            all.Add("TM", new LoadTimisoaraTaxis());
            string county = plateNumber.Substring(0, 2).ToUpper();
            if (all.ContainsKey(county))
            {
                ret = await all[county].TaxiFromPlateSqlite(plateNumber);
            }
            if (ret != null)
                return ret;
            //BUCURESTI
            county = plateNumber.Substring(0, 1).ToUpper();
            if (all.ContainsKey(county))
            {
                ret = await all[county].TaxiFromPlateSqlite(plateNumber);
            }
            if (ret != null)
                return ret;


            //search all one by one - not sure if sqlite supports threading
            foreach (var item in all)
            {
                ret = await item.Value.TaxiFromPlateSqlite(plateNumber);
                if (ret != null)
                    return ret;
            }



            return ret;
        }
        public async Task<TaxiAutorizations> GetTaxis(string city)
        {
            ILoadTaxis taxiLoader = null;
            switch (city.ToLower())
            {
                case "bucuresti":
                    taxiLoader = new LoadBucarestTaxis();
                    break;
                case "cluj":
                    taxiLoader = new LoadClujTaxis();
                    break;
                case "timisoara":
                    taxiLoader = new LoadTimisoaraTaxis();
                    break;
                case "roman":
                    taxiLoader = new LoadRoman();
                    break;
                case "bacau":
                    taxiLoader = new LoadBacauTaxis();
                    break;
                default:
                    throw new ArgumentException("only for " + string.Join(",", GetCities()));
            }
            return await taxiLoader.TaxiFromPlateSqliteAll();
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