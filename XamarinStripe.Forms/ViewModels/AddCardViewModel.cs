using System;
using System.Threading.Tasks;
using Stripe;
using XamarinStripe.Forms.Services;
using Xamarin.Forms;

namespace XamarinStripe.Forms.ViewModels {
  internal class AddCardViewModel : ViewModelBase {
    private readonly Action<PaymentMethod> _callback;
    private ImageSource _image = ImageService.Instance.CardUnknown;
    private int _length = 16;
    private string _month = "";
    private string _number = "";
    private bool _useCardFront;
    private string _verificationCode = "";
    private string _year = "";
    private string _zip = "";

    public AddCardViewModel(Action<PaymentMethod> callback) {
      DoneCommand = new Command(async () => await Done(), IsValid);
      _callback = callback;
    }

    public ImageSource CardFront => ImageService.Instance.CardFront;
    public ImageSource CardBack => ImageService.Instance.CardBack;

    public bool UseCardFront {
      get => _useCardFront;
      set => SetProperty(ref _useCardFront, value);
    }

    public ImageSource Image {
      get => _image;
      set => SetProperty(ref _image, value);
    }

    public int Length {
      get => _length;
      set => SetProperty(ref _length, value);
    }

    public string Number {
      get => _number;
      set => SetProperty(ref _number, value, UpdateCardDetails);
    }

    public string Month {
      get => _month;
      set => SetProperty(ref _month, value, DoneCommand.ChangeCanExecute);
    }

    public string Year {
      get => _year;
      set => SetProperty(ref _year, value, DoneCommand.ChangeCanExecute);
    }

    public string VerificationCode {
      get => _verificationCode;
      set => SetProperty(ref _verificationCode, value, DoneCommand.ChangeCanExecute);
    }

    public string Zip {
      get => _zip;
      set => SetProperty(ref _zip, value, DoneCommand.ChangeCanExecute);
    }

    public Command DoneCommand { get; }
    public ImageSource DoneImage => ImageService.Instance.Done;

    private bool IsValid() {
      if (Number.Length != Length) return false;

      if (Month.Length != 2 || !int.TryParse(Month, out var month)) return false;

      if (Year.Length != 2 || !int.TryParse(Year, out var year)) return false;

      if (string.IsNullOrWhiteSpace(VerificationCode) || !int.TryParse(VerificationCode, out _)) return false;

      if (string.IsNullOrWhiteSpace(Zip)) return false;

      if (month < 1 || month > 12) return false;

      year += DateTime.Now.Year / 100 * 100; // Two digit to four digit

      if (year < DateTime.Now.Year || year > DateTime.Now.AddYears(16).Year) return false;

      // TODO: Probably a little more validation here!
      return true;
    }

    private async Task Done() {
      try {
        // Create the payment method
        var paymentMethodService = new PaymentMethodService(await EphemeralService.Instance.GetClient(true));
        var paymentMethodCreateOptions = new PaymentMethodCreateOptions {
          Card = new PaymentMethodCardOptions {
            Number = Number,
            Cvc = VerificationCode,
            ExpMonth = long.Parse(Month),
            ExpYear = long.Parse(Year)
          },
          Type = "card",
          BillingDetails = new PaymentMethodBillingDetailsOptions {
            Address = new AddressOptions {
              PostalCode = Zip
            }
          }
        };
        var paymentMethod = await paymentMethodService.CreateAsync(paymentMethodCreateOptions);

        // Attach the payment method to the customer
        paymentMethodService = new PaymentMethodService(await EphemeralService.Instance.GetClient());
        var paymentMethodAttachOptions = new PaymentMethodAttachOptions {
          Customer = await EphemeralService.Instance.GetCustomerId()
        };
        paymentMethod = await paymentMethodService.AttachAsync(paymentMethod.Id, paymentMethodAttachOptions);
        _callback(paymentMethod);
        await Navigator.CardAdded();
      }
      catch (Exception ex) {
        await Navigator.ShowMessage("Error", ex.Message);
      }
    }

    private void UpdateCardDetails() {
      (Length, Image) = CardDefinitionService.Instance.DetailsFor(Number);
      DoneCommand.ChangeCanExecute();
    }
  }
}