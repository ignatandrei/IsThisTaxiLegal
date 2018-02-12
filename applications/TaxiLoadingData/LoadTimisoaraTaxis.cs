using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Threading.Tasks;
using TaxiObjects;

namespace TaxiLoadingData
{
    public class LoadTimisoaraTaxis : ILoadTaxis
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
                    cmd.CommandText = "select * from timisoara where lower([NumarInmatriculare])=@plate";
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
                    cmd.CommandText = "select * from timisoara";
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
            taxi.Location = City.FromName("Timisoara");
            taxi.NumberAutorization = empty(rd["NumarAut"]);
            taxi.State = LicenceState.Valid;
            taxi.PersonLicensedTo = new Licensee();
            taxi.PersonLicensedTo.Name = empty(rd["Nume"]);
            taxi.CarLicensed = new Car();
            taxi.CarLicensed.PlateNumber = empty(rd["NumarInmatriculare"]);
            taxi.CarLicensed.Name = empty(rd["Marca"]); 
            return taxi;
        }
    }
}
