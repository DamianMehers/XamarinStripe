using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XamarinStripe.Forms.Models;

namespace XamarinStripe.Forms.Services {
  internal class ShippingMethodService {
    private readonly ShippingMethod _fedex = new ShippingMethod(5.99F, "FedEx", "Arrives tomorrow", "fedex");

    private readonly ShippingMethod
      _fedexWorld = new ShippingMethod(20.99F, "FedEx", "Arrives tomorrow", "fedex_world");

    private readonly ShippingMethod _upsGround =
      new ShippingMethod(0, "UPS Ground", "Arrives in 3-5 days", "ups_ground");

    private readonly ShippingMethod _upsWorldwide =
      new ShippingMethod(10.99F, "UPS Worldwide Express", "Arrives in 1-3 days", "ups_worldwide");

    private ShippingMethodService() { }
    public static ShippingMethodService Instance { get; } = new ShippingMethodService();

    public async Task<(List<ShippingMethod> list, ShippingMethod preferred)> MethodsFor(string twoLetterCountryCode) {
      await Task.Delay(TimeSpan.FromSeconds(1));
      switch (twoLetterCountryCode.ToUpperInvariant()) {
        case "US":
          return (new List<ShippingMethod> {_upsGround, _fedex}, _fedex);
        case "GB":
          throw new ShippingException("Invalid Shipping Address", "We can't ship to this country.");
        default:
          return (new List<ShippingMethod> {_upsWorldwide, _fedexWorld}, _fedexWorld);
      }
    }

    public class ShippingException : Exception {
      public ShippingException(string message, string reason) : base(message) {
        Reason = reason;
      }

      public string Reason { get; }
    }
  }
}