using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VerifyTaxiApp.Interfaces;
using VerifyTaxiApp.Models;

namespace VerifyTaxiApp.Services
{
    public class SenderService : ISenderService
    {
        public string BaseUrl = "https://isthistaxilegal.apphb.com";

        public async Task<TaxiAutorization> SendNumber(string plateNumber)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = new Uri($"{BaseUrl}/api/taxi/GetTaxi?plateNumber={plateNumber}");


                    var response = await client.GetAsync(url);
                    string resultContent = await response.Content.ReadAsStringAsync();
                    TaxiAutorization taxiAutorization = JsonConvert.DeserializeObject<TaxiAutorization>(resultContent);

                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return null;
                    return taxiAutorization;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TaxiAutorization> SendPictureBase64(string base64Picture)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = new Uri($"{BaseUrl}/api/taxi/GetFromPicture");

                    var content = new StringContent(base64Picture, Encoding.UTF8);

                    var response = await client.PostAsync(url,content);
                    string resultContent = await response.Content.ReadAsStringAsync();
                    TaxiAutorization taxiAutorization = JsonConvert.DeserializeObject<TaxiAutorization>(resultContent);

                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return null;
                    return taxiAutorization;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public async Task<IEnumerable<string>> GetCities()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = new Uri($"{BaseUrl}/api/taxi/GetCities");


                    var response = await client.GetAsync(url);
                    string resultContent = await response.Content.ReadAsStringAsync();
                    IEnumerable<string> cities = JsonConvert.DeserializeObject<IEnumerable<string>>(resultContent);

                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return null;
                    return cities;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
