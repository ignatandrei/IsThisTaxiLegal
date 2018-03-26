using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyTaxiApp.Models;

namespace VerifyTaxiApp.Interfaces
{
    interface ISenderService
    {
        Task<TaxiAutorization> SendNumber(string plateNumber);
        Task<TaxiAutorization> SendPictureBase64(string base64Picture);
        Task<IEnumerable<string>> GetCities();
    }
}
