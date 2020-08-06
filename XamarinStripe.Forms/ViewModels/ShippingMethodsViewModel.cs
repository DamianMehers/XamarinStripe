using System;
using System.Collections.Generic;
using System.Linq;
using XamarinStripe.Forms.Models;
using XamarinStripe.Forms.Services;
using Xamarin.Forms;

namespace XamarinStripe.Forms.ViewModels {
  internal class ShippingMethodsViewModel : ViewModelBase {
    private bool _busy;

    public ShippingMethodsViewModel(Action shippingAddressUpdated) {
      DoneCommand = new Command(async () => {
        shippingAddressUpdated(); // Update customer with address
        await Navigator.ShippingDone();
      });
    }

    public bool Busy {
      get => _busy;
      set => SetProperty(ref _busy, value, DoneCommand.ChangeCanExecute);
    }

    public ImageSource ShippingImage => ImageService.Instance.Shipping;


    public List<ShippingMethodViewModel> Methods { get; private set; }

    public Command DoneCommand { get; set; }

    public ImageSource DoneImage => ImageService.Instance.Done;

    public void Update(List<ShippingMethod> methods, ShippingMethod selected) {
      Methods = methods.Select(m => new ShippingMethodViewModel(m, MethodSelected) {Selected = m == selected}).ToList();
    }

    private void MethodSelected(ShippingMethodViewModel selectedShippingMethodViewModel) {
      foreach (var method in Methods) method.Selected = method == selectedShippingMethodViewModel;
    }
  }
}