using System.Reflection;
using Xamarin.Forms;

namespace XamarinStripe.Forms.Services {
  internal class ImageService {
    private ImageService() { }

    public static ImageService Instance { get; } = new ImageService();

    public ImageSource Next { get; } = Load("baseline_arrow_forward_black_48dp.png");
    public ImageSource Done { get; } = Load("baseline_done_black_48dp.png");
    public ImageSource Settings { get; } = Load("Settings.scale-300.png");

    public ImageSource ApplePay { get; } = Load("stp_card_applepay.png");
    public ImageSource GooglePay { get; } = Load("GooglePay_mark_800_gray_3x.png");

    public ImageSource CardFront { get; } = Load("stp_card_form_front@3x.png");

    public ImageSource CardBack { get; } = Load("stp_card_form_back@3x.png");

    public ImageSource Amex { get; } = Load("stp_card_amex@3x.png");

    public ImageSource Diners { get; } = Load("stp_card_diners@3x.png");

    public ImageSource Discover { get; } = Load("stp_card_discover@3x.png");
    public ImageSource Jcb { get; } = Load("stp_card_jcb@3x.png");
    public ImageSource Mastercard { get; } = Load("stp_card_mastercard@3x.png");
    public ImageSource Unionpay { get; } = Load("stp_card_unionpay_en@3x.png");
    public ImageSource Visa { get; } = Load("stp_card_visa@3x.png");
    public ImageSource Shipping { get; } = Load("stp_shipping_form.png");

    public ImageSource CardError { get; } = Load("stp_card_error@3x.png");

    public ImageSource CardUnknown { get; } = Load("stp_card_unknown@3x.png");

    private static ImageSource Load(string name) {
      return ImageSource.FromResource($"XamarinStripe.Forms.Resources.{name}",
        typeof(ImageService).GetTypeInfo().Assembly);
    }
  }
}