using System.Collections.Generic;
using System.Linq;

namespace TaxiObjects
{
    public class TaxiAutorizations : List<TaxiAutorization>
    {
        public TaxiAutorization FindPlate(string plateNumber)
        {
            return this.FirstOrDefault(it => it?.CarLicensed?.PlateNumber?.ToLower() == plateNumber?.ToLower());
        }
        public TaxiAutorization FindNumber(string autNumber)
        {
            return this.FirstOrDefault(it => it?.NumberAutorization?.ToLower() == autNumber?.ToLower());
        }

    }
}
