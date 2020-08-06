using System;
using Stripe;
using XamarinStripe.Forms.Services;
using Xamarin.Forms;

namespace XamarinStripe.Forms.ViewModels {
  internal class PaymentMethodViewModel : ViewModelBase {
    private bool _selected;

    public PaymentMethodViewModel(PaymentMethod paymentMethod, Action<PaymentMethodViewModel> onSelected) {
      PaymentMethod = paymentMethod;
      SelectedCommand = new Command(() => onSelected(this));
    }

    public PaymentMethod PaymentMethod { get; }
    public ImageSource Logo => CardDefinitionService.Instance.ImageForBrand(PaymentMethod.Card.Brand);
    public string Name => CardDefinitionService.Instance.NameForBrand(PaymentMethod.Card.Brand);

    public string LastFour => PaymentMethod.Card.Last4;

    public Command SelectedCommand { get; }


    public bool Selected {
      get => _selected;
      set => SetProperty(ref _selected, value);
    }
  }
}