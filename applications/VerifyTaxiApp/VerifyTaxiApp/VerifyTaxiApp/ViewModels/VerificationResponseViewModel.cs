using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyTaxiApp.Enums;
using VerifyTaxiApp.Interfaces;
using VerifyTaxiApp.Models;
using Xamarin.Forms;

namespace VerifyTaxiApp.ViewModels
{
    public class VerificationResponseViewModel : BaseViewModel
    {
        public VerificationResponseViewModel(TaxiAutorization taxiAutorization)
        {
            try
            {
                if (taxiAutorization == null)
                {
                    responseText = "Taxiul nu a fost gasit.";
                    ImageSource = "#708090";
                    DriverName = "Verificati orasele disponibile ";
                    Car = "in sectiunea info.";
                } 
                else
                {
                    switch (taxiAutorization.State)
                    {
                        case LicenceState.Valid:
                            responseText = "Taxiul este valid";
                            ImageSource = "#3CB371";
                            DriverName = taxiAutorization.PersonLicensedTo.Name;
                            Car = taxiAutorization.CarLicensed.Name + " - " + taxiAutorization.CarLicensed.ManufacturingDate.ToString();
                            break;
                        case LicenceState.NotValid:
                            responseText = "Taxiul nu este valid";
                            ImageSource = "#B22222";
                            break;
                        case LicenceState.ToBeAnalyzed:
                            responseText = "Taxiul nu este valid";
                            ImageSource = "#B22222";
                            break;
                        default:
                            responseText = "Taxiul nu a fost gasit.Verificati orasele disponibile.";
                            ImageSource = "#FFD700";
                            DriverName = "Verificati orasele disponibile.";
                            break;
                    }

                }
            }
            catch (Exception exv)
            { }
        }

        string responseText = string.Empty;
        public string ResponseText
        {
            get { return responseText; }
            set { responseText = value; OnPropertyChanged(); }
        }

        string imageSource;
        public string ImageSource
        {
            get { return imageSource; }
            set { imageSource = value; OnPropertyChanged(); }
        }

        string driverName;
        public string DriverName
        {
            get { return driverName; }
            set { driverName = value; OnPropertyChanged(); }
        }

        string car;
        public string Car
        {
            get { return car; }
            set { car = value; OnPropertyChanged(); }
        }

        string manufacturingDate;
        public string ManufacturingDate
        {
            get { return manufacturingDate; }
            set { manufacturingDate = value; OnPropertyChanged(); }
        }

    }
}
