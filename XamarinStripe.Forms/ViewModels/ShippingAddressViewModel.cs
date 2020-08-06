using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Stripe;
using XamarinStripe.Forms.Models;
using XamarinStripe.Forms.Services;
using Xamarin.Forms;

namespace XamarinStripe.Forms.ViewModels {
  internal class ShippingAddressViewModel : ViewModelBase {
    private const string UnitedStates = "US";
    private readonly ShippingMethodsViewModel _shippingMethodsViewModel;
    private bool _busy;
    private Country _selectedCountry;

    public ShippingAddressViewModel(ShippingMethodsViewModel shippingMethodsViewModel) {
      NextCommand = new Command(async () => await Next(), () => !Busy && IsEverythingValid());
      _shippingMethodsViewModel = shippingMethodsViewModel;
      Countries = GetCountries();
    }

    internal ShippingAddress ShippingAddress { get; } = new ShippingAddress();


    public bool Busy {
      get => _busy;
      set => SetProperty(ref _busy, value, NextCommand.ChangeCanExecute);
    }

    public List<Country> Countries { get; }

    public string Name {
      get => ShippingAddress.Name;
      set
      {
        if (value == ShippingAddress.Name) return;

        ShippingAddress.Name = value;
        OnPropertyChanged();
        CheckValues();
      }
    }

    public string Address {
      get => ShippingAddress.Address;
      set
      {
        if (value == ShippingAddress.Address) return;

        ShippingAddress.Address = value;
        OnPropertyChanged();
        CheckValues();
      }
    }

    public string Apartment {
      get => ShippingAddress.Apartment;
      set
      {
        if (value == ShippingAddress.Apartment) return;

        ShippingAddress.Apartment = value;
        OnPropertyChanged();
        CheckValues();
      }
    }

    public Country SelectedCountry {
      get => _selectedCountry;
      set => SetProperty(ref _selectedCountry, value, () => {
        CheckValues();
        ShippingAddress.Country = value.Name;
      });
    }

    public string ZipCode {
      get => ShippingAddress.ZipCode;
      set
      {
        if (value == ShippingAddress.ZipCode) return;

        ShippingAddress.ZipCode = value;
        OnPropertyChanged();
        CheckValues();
      }
    }

    public string City {
      get => ShippingAddress.City;
      set
      {
        if (value == ShippingAddress.City) return;

        ShippingAddress.City = value;
        OnPropertyChanged();
        CheckValues();
      }
    }

    public string State {
      get => ShippingAddress.State;
      set
      {
        if (value == ShippingAddress.State) return;

        ShippingAddress.State = value;
        OnPropertyChanged();
        CheckValues();
      }
    }

    public string Phone {
      get => ShippingAddress.Phone;
      set
      {
        if (value == ShippingAddress.Phone) return;

        ShippingAddress.Phone = value;
        OnPropertyChanged();
        CheckValues();
      }
    }

    public Command NextCommand { get; }

    public ImageSource NextImage => ImageService.Instance.Next;

    public ImageSource ShippingImage => ImageService.Instance.Shipping;

    private List<Country> GetCountries() {
      // Derived from https://csharp.net-tutorials.com/working-with-culture-and-regions/the-regioninfo-class/
      var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
      var countries = from ci in cultures select new RegionInfo(ci.Name);

      var result = countries.Select(c => new Country(c.TwoLetterISORegionName, c.EnglishName)).OrderBy(n => n.Name)
        .Distinct().ToList();
      var us = result.Single(i => i.Code == UnitedStates);
      result.Remove(us);
      result.Insert(0, us);
      SelectedCountry = us;
      return result;
    }

    private bool IsEverythingValid() {
      // TODO: Add checks
      if (string.IsNullOrWhiteSpace(Name)) return false;

      if (string.IsNullOrWhiteSpace(Address)) return false;

      if (string.IsNullOrWhiteSpace(ZipCode)) return false;

      if (string.IsNullOrWhiteSpace(City)) return false;

      if (string.IsNullOrWhiteSpace(State)) return false;

      if (string.IsNullOrWhiteSpace(Phone)) return false;

      return true;
    }

    private async Task Next() {
      if (!IsEverythingValid()) return;

      try {
        Busy = true;

        var stripeClient = await EphemeralService.Instance.GetClient();
        var customerClient = new CustomerService(stripeClient);
        var customerUpdateOptions = new CustomerUpdateOptions {
          Name = Name,
          Address = new AddressOptions {
            Line1 = ShippingAddress.Address,
            PostalCode = ShippingAddress.ZipCode,
            City = ShippingAddress.City,
            State = ShippingAddress.State,
            Country = ShippingAddress.Country
          }
        };

        var customer =
          await customerClient.UpdateAsync(await EphemeralService.Instance.GetCustomerId(), customerUpdateOptions);
        // TODO: something with the customer?

        var (list, preferred) = await ShippingMethodService.Instance.MethodsFor(SelectedCountry.Code);
        _shippingMethodsViewModel.Update(list, preferred);
        await Navigator.ShippingMethod(_shippingMethodsViewModel);
      }
      catch (ShippingMethodService.ShippingException shippingException) {
        await Navigator.ShowMessage(shippingException.Message, shippingException.Reason);
      }
      finally {
        Busy = false;
      }
    }

    private void CheckValues() {
      NextCommand.ChangeCanExecute();
    }

    public struct Country {
      public Country(string code, string name) {
        Code = code;
        Name = name;
      }

      public string Code { get; }
      public string Name { get; }

      public override string ToString() {
        return Name;
      }
    }
  }
}