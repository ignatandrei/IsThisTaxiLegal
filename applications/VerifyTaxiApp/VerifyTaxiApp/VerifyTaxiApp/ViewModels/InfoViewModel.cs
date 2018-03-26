using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyTaxiApp.Helpers;
using VerifyTaxiApp.Interfaces;
using VerifyTaxiApp.Services;
using Xamarin.Forms;

namespace VerifyTaxiApp.ViewModels
{
    public class InfoViewModel : BaseViewModel
    {
        private SenderService _senderService;
        public ObservableRangeCollection<string> Cities { get; set; }

        public Command LoadItemsCommand { get; set; }
        public Command GoToBellonaCommand { get; set; }
        public Command GoToAdcesCommand { get; set; }


        public InfoViewModel()
        {
            _senderService = DependencyService.Get<ISenderService>() as SenderService;
            Cities = new ObservableRangeCollection<string>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            LoadItemsCommand.Execute(null);

            GoToBellonaCommand = new Command(() => Device.OpenUri(new Uri("http://www.bellonaelectronics.com/")));
            GoToAdcesCommand = new Command(() => Device.OpenUri(new Uri("http://www.adces.ro/")));
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Cities.Clear();
                var containers = await _senderService.GetCities();
                Cities.ReplaceRange(containers);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessagingCenter.Send(new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = "Unable to load cities.",
                    Cancel = "OK"
                }, "message");
            }
            finally
            {
                IsBusy = false;
                Message = string.Empty;
            }
        }
    }
}
