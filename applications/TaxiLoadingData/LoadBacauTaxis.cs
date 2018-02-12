using Microsoft.Data.Sqlite;
using System;
using System.Data;
//using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TaxiObjects;

namespace TaxiLoadingData
{
    public class LoadBacauTaxis : ILoadTaxis
    {

        public async Task<TaxiAutorization> TaxiFromPlateSqlite(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
                return null;
            plateNumber = plateNumber.ToLower();
            using (var con = new SqliteConnection())
            {
                con.ConnectionString = "Data Source=taxis.sqlite3;";

                //con.ConnectionString = "taxis.sqlite3";
                await con.OpenAsync();
                TaxiAutorization taxi = null;
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select * from bacau where lower([Nr.inmatriculare ])=@plate";
                    cmd.Parameters.AddWithValue("@plate", plateNumber);
                    using (var rd = await cmd.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            // just first record
                            if (taxi != null)
                                break;
                            taxi = From(rd);
                        }
                    }
                }

                return taxi;
            }
        }
        public async Task<TaxiAutorizations> TaxiFromPlateSqliteAll()
        {
            var all = new TaxiAutorizations();
            using (var con = new SqliteConnection())
            {
                con.ConnectionString = "Data Source=taxis.sqlite3;";

                //con.ConnectionString = "taxis.sqlite3";
                await con.OpenAsync();
                TaxiAutorization taxi = null;
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select * from bacau";
                    using (var rd = await cmd.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            // just first record

                            taxi = From(rd);
                            all.Add(taxi);
                        }
                    }
                }

                return all;
            }
        }

        TaxiAutorization From(IDataReader rd)
        {
            TaxiAutorization taxi = new TaxiAutorization();
            Func<object, string> empty = (rec) =>
            {
                if (rec == null)
                    return "";
                if (rec == DBNull.Value)
                    return "";
                string value = rec.ToString();
                if (string.IsNullOrWhiteSpace(value))
                    return "";

                return value;
            };

            taxi = new TaxiAutorization();
            taxi.Location = City.FromName("Bacau");
            taxi.NumberAutorization = empty(rd["Nr.aut.taxi "]);
            taxi.State = LicenceState.Valid;
            taxi.PersonLicensedTo = new Licensee();
            taxi.PersonLicensedTo.Name = empty(rd["Denumire transportator autorizat "]);
            taxi.CarLicensed = new Car();
            taxi.CarLicensed.PlateNumber = empty(rd["Nr.inmatriculare "]);
            taxi.CarLicensed.Name = empty(rd["Marca "]);
            return taxi;
        }

    }
}
