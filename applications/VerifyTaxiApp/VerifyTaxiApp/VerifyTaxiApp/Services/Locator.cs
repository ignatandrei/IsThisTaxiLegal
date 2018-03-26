using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyTaxiApp.ViewModels;
using VerifyTaxiApp.Views;

namespace VerifyTaxiApp.Services
{
    public class Locator
    {
        public const string MainPage = "MainView";
        public const string VerificationResponsePage = "VerificationResponsePage";
        public const string InfoPage = "InfoPage";

        static Locator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var navigation = new NavigationService();

            navigation.Configure(Locator.MainPage, typeof(MainPage));
            navigation.Configure(Locator.VerificationResponsePage, typeof(VerificationResponsePage));
            navigation.Configure(Locator.InfoPage, typeof(InfoPage));


            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<VerificationResponseViewModel>();
            SimpleIoc.Default.Register<InfoViewModel>();


            SimpleIoc.Default.Register(() => navigation);
        }

        public NavigationService NavigationService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }
    }
}
