using VerifyTaxiApp.Services;
using VerifyTaxiApp.Views;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace VerifyTaxiApp
{
    public partial class App : Application
    {
        private static Locator _locator;
        public static Locator Locator
        {
            get
            {
                return _locator ?? (_locator = new Locator());
            }
        }
        public App()
        {
            InitializeComponent();

            DependencyService.Register<SenderService>();

            SetMainPage();
        }

        public void SetMainPage()
        {
            NavigationPage navigationPage = null;
            navigationPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = (Color)Current.Resources["Primary"],
                BarTextColor = Color.White
            };
            Locator.NavigationService.Initialize(navigationPage);
            MainPage = navigationPage;

        }
    }
}
