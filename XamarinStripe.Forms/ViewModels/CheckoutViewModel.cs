using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Stripe;
using XamarinStripe.Forms.Services;
using Xamarin.Forms;

namespace XamarinStripe.Forms.ViewModels {
  internal class CheckoutViewModel : ViewModelBase {
    private readonly PaymentOptionsViewModel _paymentOptionsViewModel;
    private readonly ShippingAddressViewModel _shippingAddressViewModel;

    private readonly ShippingMethodsViewModel _shippingMethodsViewModel;
    private bool _busy;
    private bool _paymentMethodLoading;

    public CheckoutViewModel(ReadOnlyCollection<ProductViewModel> products) {
      _shippingMethodsViewModel = new ShippingMethodsViewModel(ShippingAddressUpdated);
      _shippingAddressViewModel = new ShippingAddressViewModel(_shippingMethodsViewModel);
      _paymentOptionsViewModel = new PaymentOptionsViewModel(PaymentMethodUpdated);
      SelectShippingAddressCommand = new Command(SelectShippingAddress);
      SelectPaymentOptionCommand = new Command(SelectPaymentOption);
      Products = products;
      Device.BeginInvokeOnMainThread(async () => await Load());
      BuyCommand = new Command(async () => await Buy(), CanBuy);
    }

    public ReadOnlyCollection<ProductViewModel> Products { get; }


    public bool PaymentMethodLoading {
      get => _paymentMethodLoading;
      set => SetProperty(ref _paymentMethodLoading, value, () => OnPropertyChanged(nameof(PaymentMethodLoaded)));
    }

    public bool PaymentMethodLoaded => !PaymentMethodLoading;

    public Command SelectShippingAddressCommand { get; }
    public Command SelectPaymentOptionCommand { get; }

    public float Total => Products.Sum(p => p.Price);

    private bool HaveShipTo => _shippingMethodsViewModel.Methods?.First(m => m.Selected) != null &&
                               !string.IsNullOrWhiteSpace(_shippingAddressViewModel.Name);

    public string ShipTo {
      get
      {
        var method = _shippingMethodsViewModel.Methods?.First(m => m.Selected)?.Label;
        if (method != null && !string.IsNullOrWhiteSpace(_shippingAddressViewModel.Name))
          return _shippingAddressViewModel.Name + ", " + method;

        return "Select address";
      }
    }

    private bool HavePayFrom => _paymentOptionsViewModel?.PaymentMethods.SingleOrDefault(m => m.Selected) != null;

    public string PayFrom => _paymentOptionsViewModel?.PaymentMethods.SingleOrDefault(m => m.Selected)?.Name ??
                             "Select Payment Method";

    public Command BuyCommand { get; }

    public bool Busy {
      get => _busy;
      set => SetProperty(ref _busy, value, () => BuyCommand.ChangeCanExecute());
    }

    private bool CanBuy() {
      return !Busy && HavePayFrom && HaveShipTo;
    }

    private async Task Buy() {
      try {
        Busy = true;

        var products = Products.Select(p => p.OriginalProduct).ToList();
        var address = _shippingAddressViewModel.ShippingAddress;
        var method = _paymentOptionsViewModel.PaymentMethods.SingleOrDefault(pm => pm.Selected)?.PaymentMethod;

        if (method == null) throw new Exception("Payment method unexpectedly null");

        var (secret, intent) = await EphemeralService.Instance.CreatePaymentIntent(products, address, method);

        var paymentIntentService = new PaymentIntentService(await EphemeralService.Instance.GetClient(true));

        var paymentConfirmOptions = new PaymentIntentConfirmOptions {
          ClientSecret = secret,
          Expand = new List<string> {"payment_method"},
          PaymentMethod = method.Id,
          UseStripeSdk = true,
          ReturnUrl = "payments-example://stripe-redirect"
        };
        var result = await paymentIntentService.ConfirmAsync(intent, paymentConfirmOptions);
        // TODO: Something with result?
        Busy = false;
        await Navigator.ShowMessage("Success", "Your purchase was successful!");
      }
      catch (Exception ex) {
        Busy = false;
        await Navigator.ShowMessage("Error", ex.Message);
      }
    }


    private async Task Load() {
      try {
        PaymentMethodLoading = true;
        await _paymentOptionsViewModel.Refresh();
      }
      catch (Exception ex) {
        await Navigator.ShowMessage("Error", ex.Message);
      }
      finally {
        PaymentMethodLoading = false;
      }

      OnPropertyChanged(nameof(PayFrom));
    }

    private void SelectPaymentOption() {
      Navigator.PaymentOptions(_paymentOptionsViewModel);
    }

    private void SelectShippingAddress() {
      Navigator.ShippingAddress(_shippingAddressViewModel);
    }

    public void ShippingAddressUpdated() {
      OnPropertyChanged(nameof(ShipTo));
      BuyCommand.ChangeCanExecute();
    }

    private void PaymentMethodUpdated() {
      OnPropertyChanged(nameof(PayFrom));
      BuyCommand.ChangeCanExecute();
    }
  }
}