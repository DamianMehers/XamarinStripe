using XamarinStripe.Forms.ViewModels;
using XamarinStripe.Forms.Views;
using Xamarin.Forms;

namespace XamarinStripe.Forms {
  public partial class App {
    public App() {
      InitializeComponent();

      //StripeConfiguration.ApiKey

      Navigator.Checkout = vm => MainPage.Navigation.PushAsync(new CheckoutPage {BindingContext = vm});
      Navigator.ShippingAddress = vm => MainPage.Navigation.PushAsync(new ShippingAddressPage {BindingContext = vm});
      Navigator.ShowMessage = (title, message) => MainPage.DisplayAlert(title, message, "OK");
      Navigator.ShippingMethod = vm => MainPage.Navigation.PushAsync(new ShippingMethodPage {BindingContext = vm});

      Navigator.ShippingDone = async () => {
        await MainPage.Navigation.PopAsync(false);
        await MainPage.Navigation.PopAsync(false);
      };

      Navigator.PaymentOptions = vm => MainPage.Navigation.PushAsync(new PaymentOptionsPage {BindingContext = vm});
      Navigator.AddCard = vm => MainPage.Navigation.PushAsync(new AddCardPage {BindingContext = vm});
      Navigator.CardAdded = async () => {
        await MainPage.Navigation.PopAsync(false);
        await MainPage.Navigation.PopAsync(false);
      };

      Navigator.PaymentMethodSelected = () => MainPage.Navigation.PopAsync(false);

      MainPage = new NavigationPage(new BrowseProductsPage());
    }

    protected override void OnStart() { }

    protected override void OnSleep() { }

    protected override void OnResume() { }
  }
}