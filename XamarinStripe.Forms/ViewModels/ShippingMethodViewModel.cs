using System;
using XamarinStripe.Forms.Models;
using Xamarin.Forms;

namespace XamarinStripe.Forms.ViewModels {
  internal class ShippingMethodViewModel : ViewModelBase {
    private bool _selected;

    public ShippingMethodViewModel(ShippingMethod shippingMethod, Action<ShippingMethodViewModel> selectedCallback) {
      ShippingMethod = shippingMethod;
      SelectedCommand = new Command(() => selectedCallback(this));
    }

    public ShippingMethod ShippingMethod { get; }

    public string Label => ShippingMethod.Label;
    public string Detail => ShippingMethod.Detail;
    public float Price => ShippingMethod.Amount;


    public bool Selected {
      get => _selected;
      set => SetProperty(ref _selected, value);
    }

    public Command SelectedCommand { get; }
  }
}