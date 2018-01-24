using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TaxiObjects;

namespace TaxiLoadingData
{
    class BucarestCSV:IValidatableObject
    {
        string Line;
        public BucarestCSV(string line)
        {
            Line = line;
            
        }
        
        public IEnumerable<ValidationResult> Parse()
        {
            string message = null;
            try
            {
                var splitLine = Line.Split('|');
                if (splitLine.Length < 7)
                    throw new ArgumentException("line not enough separated " + Line);
                NrAutTaxi = splitLine[0];
                StareAutTaxi = splitLine[1];
                NumeTransportator = splitLine[2];
                MarcaAuto = splitLine[3];
                var year = splitLine[4];
                if (!string.IsNullOrWhiteSpace(year))
                {
                    if (year.Length > 4)
                        year = year.Substring(0, 4);

                    AnFabricatieAuto = int.Parse(year);
                }
                NrInmatriculareAuto = splitLine[5];
                DateTime expiration = DateTime.MinValue;
                string date = splitLine[6];
                if (!string.IsNullOrWhiteSpace(date))
                {
                    if (date == "27.02.202" || date == "27.02.020")
                    {
                        expiration = new DateTime(2020, 02, 27);
                    }
                    else if (!DateTime.TryParseExact(date, "dd.mm.yyyy", null, DateTimeStyles.None, out expiration))
                    {
                        if (!DateTime.TryParseExact(date, "d.mm.yyyy", null, DateTimeStyles.None, out expiration))
                        {

                            throw new ArgumentException("not a valid date :" + splitLine[6] + " from line" + Line);
                        }

                    }
                }

                ExpirareValabilitate = expiration;
                if (splitLine.Length > 7)
                    Observatii = splitLine[7];
            }
            catch(Exception ex)
            {
                message = ex.Message;
            }
            
            if(message != null)
                yield return new ValidationResult(message);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return Parse();
        }

        public string NrAutTaxi { get; private set; }
        public string StareAutTaxi { get; private set; }
        public string NumeTransportator { get; private set; }
        public string MarcaAuto { get; private set; }

        public int? AnFabricatieAuto { get; private set; }
        public string NrInmatriculareAuto { get; private set; }
        public DateTime ExpirareValabilitate { get; private set; }

        public string Observatii { get; private set; }
        
    }
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
        public async Task<TaxiAutorization> TaxiFromPlate(string plateNumber)
        {
            using(var con =new SQLiteConnection())
            {
                BucarestCSV bucarestCSV = null;
                con.ConnectionString = "taxis.sqlite3";
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select * from bucuresti where NrInmatriculareAuto=@plate";
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
                            line += empty(rd["Nr.Aut.Taxi"]);
                            line +="|";
                            line += empty(rd["Stare Aut.Taxi"]);
                            line += "|";
                            line += empty(rd["Nume Transportator"]);
                            line += "|";
                            line += empty(rd["Marca Auto"]);
                            line += "|";
                            line += empty(rd["An Fabricatie Auto"]);
                            line += "|";
                            line += empty(rd["Nr.Inmatriculare Auto"]);
                            line += "|";
                            line += empty(rd["Expirare Valabilitate"]);
                            line += "|";
                            line += empty(rd["Observatii"]); 
                            bucarestCSV = new BucarestCSV(line);
                            
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
