using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TaxiObjects;

namespace TaxiLoadingData
{
    public class LoadClujTaxis
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
                    cmd.CommandText = "select * from cluj where lower([ Nr. auto ])=@plate";
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
                    cmd.CommandText = "select * from cluj";
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
            taxi.Location = City.FromName("Cluj");
            taxi.NumberAutorization = empty(rd["Nr. autoriz. TAXI "]);
            taxi.State = LicenceState.Valid;
            taxi.PersonLicensedTo = new Licensee();
            taxi.PersonLicensedTo.Name = empty(rd["Transportator autorizat"]);
            taxi.CarLicensed = new Car();
            taxi.CarLicensed.PlateNumber = empty(rd[" Nr. auto "]);
            return taxi;
        }
    }
}
