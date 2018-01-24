using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TaxiObjects;

namespace TaxiLoadingData
{
    public class LoadBucarestTaxis
    {

        private string[] LinesFromCSV()
        {
            if (File.Exists("Bucuresti.csv"))
                return File.ReadAllLines("Bucuresti.csv");

            // download from web
            var req = WebRequest.CreateHttp("https://raw.githubusercontent.com/ignatandrei/IsThisTaxiLegal/master/datas/Bucuresti.csv");
            using (var resp = req.GetResponse()) {
                using(var st = resp.GetResponseStream())
                {
                    using(var data= new StreamReader(st))
                    {
                        var lines = data.ReadToEnd();
                        return lines.Split('\n');
                    }
                }
            }
            


        }
        public async Task DownloadDatabaseSqlLite()
        {
            var url ="https://raw.githubusercontent.com/ignatandrei/IsThisTaxiLegal/master/datas/taxis.sqlite3";
            using (var client = new HttpClient())
            {

                using (var result = await client.GetAsync(url))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        var bytes=await result.Content.ReadAsByteArrayAsync();
                        File.WriteAllBytes("taxis.sqlite3",bytes);
                    }

                }
            }
        }
        public async Task<TaxiAutorizations> TaxiFromPlateSqliteAll ()
        {
            TaxiAutorizations ret = new TaxiAutorizations();
            using (var con = new SQLiteConnection())
            {
                con.ConnectionString = "Data Source=taxis.sqlite3;Version=3;UseUTF16Encoding=True;";

                
                await con.OpenAsync();
               
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select * from bucuresti";                    
                    using (var rd = await cmd.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            // just first record
                            
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
                            string line = "";
                            line += empty(rd["Nr. Aut. Taxi"]);
                            line += "|";
                            line += empty(rd["Stare Aut. Taxi"]);
                            line += "|";
                            line += empty(rd["Nume Transportator"]);
                            line += "|";
                            line += empty(rd["Marca Auto"]);
                            line += "|";
                            line += empty(rd["An Fabricatie Auto"]);
                            line += "|";
                            line += empty(rd["Nr. Inmatriculare Auto"]);
                            line += "|";
                            line += empty(rd["Expirare Valabilitate"]);
                            line += "|";
                            line += empty(rd["Observatii"]);
                            var bucarestCSV = new BucarestCSV(line);
                            var err = bucarestCSV.Parse().FirstOrDefault();
                            if (err != null)
                                throw new ArgumentException($"line:{line} ex:{ err.ErrorMessage}");
                            ret.Add(FromBucarestCSV(bucarestCSV));
                        }
                    }
                }

                return ret;
            }
        }
            public async Task<TaxiAutorization> TaxiFromPlateSqlite(string plateNumber)
        {
            using(var con =new SQLiteConnection())
            {
                con.ConnectionString = "Data Source=taxis.sqlite3;Version=3;UseUTF16Encoding=True;";
                
                //con.ConnectionString = "taxis.sqlite3";
                await con.OpenAsync();
                BucarestCSV bucarestCSV = null;
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select * from bucuresti where [Nr. Inmatriculare Auto]=@plate";
                    cmd.Parameters.AddWithValue("@plate", plateNumber);
                    using(var rd=await cmd.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            // just first record
                            if (bucarestCSV != null)
                                break;
                            Func<object, string> empty = (rec) =>
                            {
                                if(rec==null)
                                    return "";
                                if (rec == DBNull.Value)
                                    return "";
                                string value = rec.ToString();
                                if (string.IsNullOrWhiteSpace(value))
                                    return "";
                                
                                return value;
                            };
                            string line = "";
                            line += empty(rd["Nr. Aut. Taxi"]);
                            line +="|";
                            line += empty(rd["Stare Aut. Taxi"]);
                            line += "|";
                            line += empty(rd["Nume Transportator"]);
                            line += "|";
                            line += empty(rd["Marca Auto"]);
                            line += "|";
                            line += empty(rd["An Fabricatie Auto"]);
                            line += "|";
                            line += empty(rd["Nr. Inmatriculare Auto"]);
                            line += "|";
                            line += empty(rd["Expirare Valabilitate"]);
                            line += "|";
                            line += empty(rd["Observatii"]); 
                            bucarestCSV = new BucarestCSV(line);
                            var err = bucarestCSV.Parse().FirstOrDefault();
                            if (err != null)
                                throw new ArgumentException($"line:{line} ex:{ err.ErrorMessage}");
                        }
                    }
                }
                
                return FromBucarestCSV(bucarestCSV);
            }
        }
        private TaxiAutorization FromBucarestCSV(BucarestCSV bucarestLine)
        {
            if (bucarestLine == null)
                return null;
            
            var city = City.FromName("Bucarest");
            var taxi = new TaxiAutorization();
            taxi.Location = city;
            taxi.NumberAutorization = bucarestLine.NrAutTaxi;
            taxi.OtherDetails = bucarestLine.Observatii;
            var state = bucarestLine.StareAutTaxi;
            switch (state)
            {
                case var s when s.Contains("NEVALIDA"):
                    taxi.State = LicenceState.NotValid;
                    break;
                case var s when s.Contains("VALIDA"):
                    taxi.State = LicenceState.Valid;
                    break;
                case var s when s.Contains("ANALIZA"):
                    taxi.State = LicenceState.ToBeAnalyzed;
                    break;
                default:
                    taxi.State = LicenceState.Unknown;
                    break;
            }

            var car = new Car();
            car.ManufacturingDate = bucarestLine.AnFabricatieAuto;
            car.Name = bucarestLine.MarcaAuto;
            car.PlateNumber = bucarestLine.NrInmatriculareAuto;

            taxi.CarLicensed = car;
            taxi.PersonLicensedTo = new Licensee()
            {
                Name = bucarestLine.NumeTransportator
            };
            return taxi;
        }
        
            /// <summary>
            /// returns taxis and not parsed successfully lines
            /// </summary>
            /// <returns></returns>
            public Tuple<TaxiAutorizations, string[]> TaxisFromCSV()
        {
           
            var taxis = new TaxiAutorizations();
            var lines = LinesFromCSV();
            var records = lines
                .Where(it=>!string.IsNullOrWhiteSpace(it))
                .Select(it => new { line=it,bucTaxi=new BucarestCSV(it) })
                .Select(t => new { t.line,t.bucTaxi,HasError=t.bucTaxi.Parse().Any()})
                .ToArray();


            var errorsLines = records
                .Where(it => it.HasError)
                .Select(it => it.line)
                .ToArray();
            var recordsNoErrors=records
                .Where(it => !it.HasError)
                .Select(it => it.bucTaxi)
                .ToArray();
            foreach(var record in recordsNoErrors)
            {
                
                taxis.Add(FromBucarestCSV(record));

            }
            return Tuple.Create(taxis, errorsLines);
        } 

    }
}
