using Calories.App.Managers;
using Calories.App.Services;
using Xamarin.Forms;

namespace Calories.App
{
    public partial class App : Application
    {
        public App()
        {
            // Registered with simoncorcos.ing@gmail.com (W***8)
            // https://help.syncfusion.com/common/essential-studio/licensing/license-key
            // https://www.syncfusion.com/account/downloads
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
                "MjE0NjM2QDMxMzcyZTM0MmUzMGUvakcvUDFHdWl3SGJKbEtRbzJla21ES3FvTWQ5ekJ6aURpdGQ0VmN0YWc9"
            );

            InitializeComponent();

            if (!Injector.IsInitialized)
            {
                Injector.Bind<IHttpClient>().ToConstant(new CoreHttpClient());

                // Register classes decorated with the [Injector.Singleton] attribute
                Injector.GetSingletonsIn(typeof(App).Assembly);
            }

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            if (!Injector.IsInitialized)
            {
                Injector.PerformEagerActivations();

                Injector.Get<AppManager>().InitializeApplication();

                Injector.IsInitialized = true;
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
