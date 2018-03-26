using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VerifyTaxiApp.Enums;
using VerifyTaxiApp.Helpers;
using VerifyTaxiApp.Interfaces;
using VerifyTaxiApp.Models;
using VerifyTaxiApp.Services;
using Xamarin.Forms;

namespace VerifyTaxiApp.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public MainPageViewModel()
        {
            _senderService = DependencyService.Get<ISenderService>() as SenderService;
        }
        string plateNumber = string.Empty;
        public string PlateNumber
        {
            get { return plateNumber; }
            set { plateNumber = value; OnPropertyChanged(); }
        }

        public ICommand SendNumberCommand => new Command(async () => await Send());
        public ICommand TakePictureCommand => new Command(async () => await OpenCamera());
        public ICommand PickPictureCommand => new Command(async () => await PickPicture());
        public ICommand GoToInfoCommand => new Command(() => GoToInfo());

        
        private SenderService _senderService;


        private async Task PickPicture()
        {
            //if (IsBusy)
            //    return;
            //try
            //{
            //    IsBusy = true;
            //    Message = "Loading...";

            //    if (!CrossMedia.Current.IsPickPhotoSupported)
            //    {
            //        await App.Current.MainPage.DisplayAlert("Photos Not Supported", "Permission not granted to photos.", "OK");
            //        return;
            //    }
            //    var file = await CrossMedia.Current.PickPhotoAsync();


            //    if (file == null)
            //        return;

            //    MainImage = ImageSource.FromStream(() =>
            //    {
            //        var stream = file.GetStream();
            //        file.Dispose();
            //        return stream;
            //    });

            //    ImageBytes = null;
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        file.GetStream().CopyTo(memoryStream);
            //        ImageBytes = memoryStream.ToArray();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    IsBusy = false;
            //    Message = string.Empty;
            //}
        }

        private async Task OpenCamera()
        {
            //if (IsBusy)
            //    return;
            //Plugin.Media.Abstractions.MediaFile file = null;
            //try
            //{
            //    IsBusy = true;
            //    Message = "Loading...";

            //    await CrossMedia.Current.Initialize();

            //    if (!CrossMedia.Current.IsCameraAvailable
            //        || !CrossMedia.Current.IsTakePhotoSupported)
            //    {
            //        await App.Current.MainPage.DisplayAlert("No Camera", "No camera available", "OK");
            //        return;
            //    }


            //    file = await CrossMedia.Current.TakePhotoAsync(
            //        new Plugin.Media.Abstractions.StoreCameraMediaOptions
            //        {
            //            SaveToAlbum = true
            //        });

            //    if (file == null)
            //        return;

            //    MainImage = ImageSource.FromStream(() =>
            //    {
            //        var stream = file.GetStream();
            //        file.Dispose();
            //        return stream;
            //    });

            //    ImageBytes = null;
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        file.GetStream().CopyTo(memoryStream);
            //        ImageBytes = memoryStream.ToArray();
            //    }
                

            //}
            //catch (Exception exc)
            //{
            //    throw exc;
            //}
            //finally
            //{
            //    IsBusy = false;
            //    Message = string.Empty;
            //}
        }


        public byte[] ImageBytes { get; set; }
        ImageSource mainImage = string.Empty;
        public ImageSource MainImage
        {
            get { return mainImage; }
            set { mainImage = value; OnPropertyChanged(); }
        }


       
        private async Task Send()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrEmpty(plateNumber))
                return;

            try
            {
                IsBusy = true;
                Message = "Searching...";

                plateNumber = plateNumber.Replace(" ", "");
                plateNumber = plateNumber.Replace("-", "");

                TaxiAutorization taxiAutorization = null;
             //   if (!string.IsNullOrEmpty(plateNumber))
                    taxiAutorization = await _senderService.SendNumber(plateNumber);
             //   else if (ImageBytes != null)
             //       taxiAutorization = await _senderService.SendPictureBase64(Convert.ToBase64String(ImageBytes));
                GoToNextPage(taxiAutorization);


            }
            catch (Exception ex)
            {
                ;
            }
            finally
            {
                IsBusy = false;
                Message = string.Empty;
            }
        }

        private void GoToNextPage(TaxiAutorization taxiAutorization)
        {
            if (taxiAutorization == null)
                App.Locator.NavigationService.NavigateTo(Locator.VerificationResponsePage, null);
            else
            {
                //  MessagingCenter.Send(this, "Transition", TransitionType.Scale);
                App.Locator.NavigationService.NavigateTo(Locator.VerificationResponsePage, taxiAutorization);
            }
        }

        private void GoToInfo()
        {
            App.Locator.NavigationService.NavigateTo(Locator.InfoPage);
        }

    }
}
