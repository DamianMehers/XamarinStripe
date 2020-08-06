namespace XamarinStripe.Forms.Models {
  internal class ShippingMethod {
    public ShippingMethod(float amount, string label, string detail, string identifier) {
      Amount = amount;
      Label = label;
      Detail = detail;
      Identifier = identifier;
    }

    public float Amount { get; }
    public string Label { get; }
    public string Detail { get; }
    public string Identifier { get; }
  }
}