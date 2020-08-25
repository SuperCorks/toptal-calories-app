
using Android.OS;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Content.PM;

using Calories.App.Adapters;
using Calories.App.Droid.Adapters;
using Calories.App.Entities.Authentication;
using Calories.App.Droid.Util;

[assembly: Application(UsesCleartextTraffic = true)] // To allow non https calls
namespace Calories.App.Droid
{
    [Activity(Label = "Calories", 
        Icon = "@mipmap/icon", 
        Theme = "@style/MainTheme", 
        LaunchMode = LaunchMode.SingleTask, 
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
    )]
    [IntentFilter( // For OAuth browser callback
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "com.toptal.calories.app",
        DataHost = "supercorks.auth0.com",
        DataPathPrefix = "/android/com.toptal.calories.app/callback")
    ]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);

            NoDefaultPadding.Init();
            Syncfusion.XForms.Android.PopupLayout.SfPopupLayoutRenderer.Init();

            if (!Injector.IsInitialized)
            {
                // StringValueStore dependencies & initialization
                Akavache.Registrations.Start("com.toptal.calories.app");
                Akavache.BlobCache.ApplicationName = "com.toptal.calories.app";
                Injector.Bind<StringValueStore>().ToSelf().InSingletonScope();

                Injector.Bind<IOAuthAdapter>().ToConstant(new DroidOAuthAdapter(new OAuthOptions(
                    domain: "supercorks.auth0.com",
                    clientId: "0afCrwDRG7EEVYa1n5DD4YmJ0JPoLoST",
                    scope: "openid email"
                )));
            }

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            Auth0.OidcClient.ActivityMediator.Instance.Send(intent.DataString);
        }
    }
}