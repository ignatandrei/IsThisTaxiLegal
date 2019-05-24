using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace TaxiLoadingData
{
    class BucarestCSV :IValidatableObject
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
                    else if(date == "8.06.202")
                    {
                        expiration = new DateTime(2020, 06, 08);
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
}
