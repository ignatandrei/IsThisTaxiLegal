using VerifyTaxiApp.Helpers;
using VerifyTaxiApp.Models;
using VerifyTaxiApp.Services;

using Xamarin.Forms;

namespace VerifyTaxiApp.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        /// <summary>
        /// Get the azure service instance
        /// </summary>
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        /// <summary>
        /// Private backing field to hold the title
        /// </summary>
        string title = string.Empty;
        /// <summary>
        /// Public property to set and get the title of the item
        /// </summary>
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        string message = string.Empty;
        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged(); }
        }

        private bool backButtonExists = false;
        public bool BackButtonExists
        {
            get
            {
                if (Device.RuntimePlatform == Device.Android)
                    return false;
                else
                    return true;
            }
            set { backButtonExists = value; OnPropertyChanged(); }
        }
    }
}

