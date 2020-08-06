using System;
using System.Threading.Tasks;

namespace XamarinStripe.Forms.ViewModels {
  internal static class Navigator {
    internal static Func<CheckoutViewModel, Task> Checkout { get; set; }
    internal static Func<ShippingAddressViewModel, Task> ShippingAddress { get; set; }
    internal static Func<ShippingMethodsViewModel, Task> ShippingMethod { get; set; }
    internal static Func<string, string, Task> ShowMessage { get; set; }

    internal static Func<PaymentOptionsViewModel, Task> PaymentOptions { get; set; }

    internal static Func<AddCardViewModel, Task> AddCard { get; set; }

    internal static Func<Task> ShippingDone { get; set; }

    public static Func<Task> CardAdded { get; set; }

    public static Func<Task> PaymentMethodSelected { get; set; }
  }
}