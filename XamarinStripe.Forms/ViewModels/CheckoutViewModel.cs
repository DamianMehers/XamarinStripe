using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
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



    public class UseStripeSdk {
      public class DirectoryServerEncryptionKind {

        [JsonProperty("directory_server_id")] public string DirectoryServerId { get; set; }

        [JsonProperty("algorithm")] public string Algorithm { get; set; }

        [JsonProperty("certificate")] public string Certificate { get; set; }

        [JsonProperty("root_certificate_authorities")]
        public IList<string> RootCertificateAuthorities { get; set; }
      }

      [JsonProperty("type")] public string Type { get; set; }

      [JsonProperty("three_d_secure_2_source")]
      public string ThreeDSecure2Source { get; set; }

      [JsonProperty("directory_server_name")]
      public string DirectoryServerName { get; set; }

      [JsonProperty("server_transaction_id")]
      public string ServerTransactionId { get; set; }

      [JsonProperty("three_ds_method_url")] public string ThreeDsMethodUrl { get; set; }

      [JsonProperty("three_ds_optimizations")]
      public string ThreeDsOptimizations { get; set; }

      [JsonProperty("directory_server_encryption")]
      public DirectoryServerEncryptionKind DirectoryServerEncryption { get; set; }
    }

    public class Example
    {

      [JsonProperty("type")]
      public string Type { get; set; }

      [JsonProperty("use_stripe_sdk")]
      public UseStripeSdk UseStripeSdk { get; set; }
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
        switch (result.NextAction?.Type) {
          case null:
            Busy = false;
            await Navigator.ShowMessage("Success", "Your purchase was successful!");
            break; // All good

          case "stripe_3ds2_fingerprint":
          default:
            throw new NotImplementedException($"Not implemented: Can't handle {result.NextAction.Type}");
        }
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