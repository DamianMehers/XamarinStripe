using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using XamarinStripe.Forms;
using Xamarin.Forms.Platform.Android;
using Platform = Xamarin.Essentials.Platform;

namespace XamarinStripe.Droid {
  [Activity(Label = "XamarinStripe", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
  public class MainActivity : FormsAppCompatActivity {
    protected override void OnCreate(Bundle savedInstanceState) {
      TabLayoutResource = Resource.Layout.Tabbar;
      ToolbarResource = Resource.Layout.Toolbar;

      base.OnCreate(savedInstanceState);

      Platform.Init(this, savedInstanceState);
      Xamarin.Forms.Forms.Init(this, savedInstanceState);
      LoadApplication(new App());
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
      [GeneratedEnum] Permission[] grantResults) {
      Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

      base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
  }
}