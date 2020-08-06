using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Stripe;
using XamarinStripe.Forms.Services;
using Xamarin.Forms;

namespace XamarinStripe.Forms.ViewModels {
  internal class PaymentOptionsViewModel : ViewModelBase {
    private readonly Action _methodSelectedCallback;
    private Customer _customer;


    public PaymentOptionsViewModel(Action methodSelectedCallback) {
      //if (DeviceInfo.Platform == DevicePlatform.iOS) {
      //  PaymentMethods.Add(new PaymentMethodViewModel(new PaymentMethod { Card = new PaymentMethodCard { Brand = "Apple Pay"}}));
      //} else if (DeviceInfo.Platform == DevicePlatform.Android) {
      //  PaymentMethods.Add(new PaymentMethodViewModel(new PaymentMethod { Card = new PaymentMethodCard { Brand = "Google Pay" } }));
      //}

      Device.BeginInvokeOnMainThread(async () => await Refresh());
      AddCardCommand = new Command(AddCard);
      _methodSelectedCallback = methodSelectedCallback;
    }

    public ObservableCollection<PaymentMethodViewModel> PaymentMethods { get; } =
      new ObservableCollection<PaymentMethodViewModel>();


    public bool Busy { get; set; }

    public string SampleText { get; } = @"The sample backend attaches some test cards:

• 4242 4242 4242 4242
    A default VISA card.

• 4000 0000 0000 3220
    Use this to test 3D Secure 2 authentication.

See https://stripe.com/docs/testing.";

    public ImageSource CardImage => ImageService.Instance.CardFront;

    public Command AddCardCommand { get; }

    private void AddCard() {
      Navigator.AddCard(new AddCardViewModel(CardAdded));
    }

    private void CardAdded(PaymentMethod paymentMethod) {
      var paymentMethodViewModel = new PaymentMethodViewModel(paymentMethod, PaymentMethodSelected);
      PaymentMethods.Add(paymentMethodViewModel);
      SelectPaymentMethod(paymentMethodViewModel);
      _methodSelectedCallback();
    }

    private void SelectPaymentMethod(PaymentMethodViewModel selectedPaymentMethodViewModel) {
      foreach (var paymentMethodViewModel in PaymentMethods)
        paymentMethodViewModel.Selected = paymentMethodViewModel == selectedPaymentMethodViewModel;
    }

    private void PaymentMethodSelected(PaymentMethodViewModel selectedPaymentMethodViewModel) {
      SelectPaymentMethod(selectedPaymentMethodViewModel);
      _methodSelectedCallback();
      Navigator.PaymentMethodSelected();
    }


    public async Task Refresh() {
      var selectedPaymentMethodId = PaymentMethods.FirstOrDefault(p => p.Selected)?.PaymentMethod.Id;
      PaymentMethods.Clear();
      try {
        var stripeClient = await EphemeralService.Instance.GetClient();
        var customerId = await EphemeralService.Instance.GetCustomerId();
        var customerService = new CustomerService(stripeClient);
        _customer = await customerService.GetAsync(customerId);


        var paymentMethodService = new PaymentMethodService(stripeClient);
        var paymentMethodListOptions = new PaymentMethodListOptions {
          Customer = _customer.Id,
          Type = "card"
        };
        var methods = await paymentMethodService.ListAsync(paymentMethodListOptions);
        foreach (var paymentMethod in methods)
          PaymentMethods.Add(new PaymentMethodViewModel(paymentMethod, PaymentMethodSelected));

        if (PaymentMethods.Any()) {
          var toSelect = PaymentMethods.FirstOrDefault(pm => pm.PaymentMethod.Id == selectedPaymentMethodId) ??
                         PaymentMethods.First();
          SelectPaymentMethod(toSelect);
        }
      }
      catch (Exception ex) {
        await Navigator.ShowMessage("Error", ex.Message);
      }
    }
  }
}