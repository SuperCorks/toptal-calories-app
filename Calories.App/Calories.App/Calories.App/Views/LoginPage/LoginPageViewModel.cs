using Calories.App.Managers;
using Calories.App.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Calories.App.Views.LoginPage
{
    public class LoginPageViewModel : PropertyChangedNotifier
    {

        public bool IsLoggingIn { get; set; } = false;
        public bool ShowLoginButton { get; set; }

        public ICommand LoginCommand { get; }

        private AppModel AppModel => Injector.Get<AppModel>();
        private AppManager AppManager => Injector.Get<AppManager>();

        public LoginPageViewModel()
        {
            this.LoginCommand = AppManager.SafeCommand(async () =>
            {
                await AppManager.Login();
            });

            AppModel.PropertyChanged += (s, a) =>
            {
                this.IsLoggingIn = AppModel.IsLoggingIn;

                this.ShowLoginButton = !IsLoggingIn && AppModel.CurrentUser == null;
            };
        }
    }
}
