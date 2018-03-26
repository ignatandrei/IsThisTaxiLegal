using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyTaxiApp.Controls;
using VerifyTaxiApp.Enums;
using VerifyTaxiApp.Services;
using VerifyTaxiApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VerifyTaxiApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
           // BindingContext = new MainPageViewModel();
        }

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    MessagingCenter.Subscribe<MainPageViewModel, TransitionType>(this, "Transition", (sender, arg) =>
        //    {
        //        var transitionType = (TransitionType)arg;
        //        MainPageViewModel viewModel = sender as MainPageViewModel;
        //        TransitionNavigationPage transitionNavigationPage = new TransitionNavigationPage();
        //        transitionNavigationPage = Parent as TransitionNavigationPage;

        //        if (transitionNavigationPage != null)
        //        {
        //            transitionNavigationPage.TransitionType = transitionType;
        //            App.Locator.NavigationService.NavigateTo(Locator.VerificationResponsePage, sender.TaxiAutorization);
        //        }
        //    });
        //}

        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();

        //    MessagingCenter.Unsubscribe<MainPageViewModel, TransitionType>(this, "Transition");
        //}
    }
}