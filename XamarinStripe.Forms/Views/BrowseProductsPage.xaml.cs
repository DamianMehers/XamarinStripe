using System.ComponentModel;
using XamarinStripe.Forms.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinStripe.Forms.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class BrowseProductsPage {
    private readonly BrowseProductsViewModel _viewModel;

    public BrowseProductsPage() {
      InitializeComponent();
      if (BindingContext is BrowseProductsViewModel vm) {
        _viewModel = vm;
        _viewModel.PropertyChanged += BindingContextPropertyChanged;
      }
    }

    private void BindingContextPropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName != nameof(BrowseProductsViewModel.Total)) return;

      var isShowing = BuyFrame.IsVisible;
      var shouldShow = _viewModel.Total > 0;

      if (isShowing == shouldShow) return;

      Device.BeginInvokeOnMainThread(async () => {
        if (shouldShow) {
          BuyFrame.TranslationY = 50;
          BuyFrame.IsVisible = true;
          await BuyFrame.TranslateTo(BuyFrame.X, 0);
        }
        else {
          await BuyFrame.TranslateTo(BuyFrame.X, 50);
          BuyFrame.IsVisible = false;
        }
      });
    }
  }
}