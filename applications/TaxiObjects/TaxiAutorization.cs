using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace TaxiObjects
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
