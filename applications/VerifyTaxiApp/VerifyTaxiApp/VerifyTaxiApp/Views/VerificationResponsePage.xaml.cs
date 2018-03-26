using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using VerifyTaxiApp.ViewModels;
using VerifyTaxiApp.Models;

namespace VerifyTaxiApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VerificationResponsePage : ContentPage
	{

        VerificationResponseViewModel viewModel;
        public VerificationResponsePage()
        {
            InitializeComponent();

            BindingContext = viewModel = new VerificationResponseViewModel(null);
        }

        public VerificationResponsePage (TaxiAutorization parameter)
		{
			InitializeComponent ();

            BindingContext = viewModel = new VerificationResponseViewModel(parameter);
        }


    }
}