using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyTaxiApp.Enums;

namespace VerifyTaxiApp.Models
{
    public class TaxiAutorization
    {
        public string NumberAutorization { get; set; }
        public Licensee PersonLicensedTo { get; set; }
        public Car CarLicensed { get; set; }
        public LicenceState State { get; set; }
        public string OtherDetails { get; set; }
        public City Location { get; set; }
    }
}
