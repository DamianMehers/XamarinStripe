using System.Collections.Generic;
using System.Linq;
using XamarinStripe.Forms.Services;
using Xamarin.Forms;

namespace XamarinStripe.Forms.ViewModels {
  internal class BrowseProductsViewModel : ViewModelBase {
    public BrowseProductsViewModel() {
      ProductsAndPrices = ProductsDataStoreService.Instance.Items.Select(p => new ProductViewModel(p, ProductToggled))
        .ToList();
      BuyNowCommand = new Command(BuyNow);
    }

    public IEnumerable<ProductViewModel> Selected => from productAndPrice in ProductsAndPrices
      where productAndPrice.Selected
      select productAndPrice;

    public float Total => Selected.Sum(p => p.Price);

    public List<ProductViewModel> ProductsAndPrices { get; }

    public Command BuyNowCommand { get; }


    private void BuyNow() {
      Navigator.Checkout(new CheckoutViewModel(Selected.ToList().AsReadOnly()));
    }


    private void ProductToggled(ProductViewModel productViewModel) {
      productViewModel.Selected = !productViewModel.Selected;
      OnPropertyChanged(nameof(Total));
    }
  }
}