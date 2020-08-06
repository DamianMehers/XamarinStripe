using XamarinStripe.Forms.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinStripe.Forms.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class AddCardPage {
    private bool _back;

    public AddCardPage() {
      InitializeComponent();
      HeroImage.Source = ImageService.Instance.CardFront;
    }

    private void FieldFocused(object sender, FocusEventArgs e) {
      var oldValue = _back;
      _back = e.IsFocused && e.VisualElement == VerificationCode;

      if (oldValue == _back) return;

      var newImage = _back ? ImageService.Instance.CardBack : ImageService.Instance.CardFront;

      var animation = new Animation();
      var rotateAnimation1 = new Animation(r => HeroImage.RotationY = r, 0, 90, finished: () =>
        HeroImage.Source = newImage);
      var rotateAnimation2 = new Animation(r => HeroImage.RotationY = r, 90, 0);
      animation.Add(0, 0.5, rotateAnimation1);
      animation.Add(0.5, 1, rotateAnimation2);
      animation.Commit(this, "rotateCard");
    }
  }
}