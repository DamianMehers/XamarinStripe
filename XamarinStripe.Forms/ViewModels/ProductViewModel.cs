using System;
using XamarinStripe.Forms.Models;
using Xamarin.Forms;

namespace XamarinStripe.Forms.ViewModels {
  public class ProductViewModel : ViewModelBase {
    private bool _selected;

    public ProductViewModel(Product product, Action<ProductViewModel> toggle) {
      OriginalProduct = product;
      ToggleCommand = new Command(() => toggle(this));
    }

    internal Product OriginalProduct { get; }

    public bool Selected {
      get => _selected;
      set => SetProperty(ref _selected, value);
    }

    public string Emoji => OriginalProduct.Emoji;
    public float Price => OriginalProduct.Price / 100F;

    public string Name => OriginalProduct.Name;

    public Command ToggleCommand { get; }
  }
}