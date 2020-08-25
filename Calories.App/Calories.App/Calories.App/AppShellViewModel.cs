using Calories.App.Entities.Users;
using Calories.App.Managers;
using Calories.App.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Calories.App
{
    class AppShellViewModel : PropertyChangedNotifier
    {
        public string Username { get; set; }

        public bool UserIsLoggedIn => this.CurrentUser != null;
        public User CurrentUser { get; set; }

        public ICommand LogoutCommand { get; }

        private AppModel AppModel => Injector.Get<AppModel>();
        private AppManager AppManager => Injector.Get<AppManager>();
        private AuthenticationManager AuthenticationManager => Injector.Get<AuthenticationManager>();

        public AppShellViewModel()
        {
            AppModel.PropertyChanged += (s, a) => this.Update();

            this.LogoutCommand = AppManager.SafeCommand(async () =>
            {
                await AuthenticationManager.LogOut();
                AppModel.Reset();
            });

            this.Update();
        }

        private void Update()
        {
            this.CurrentUser = AppModel.CurrentUser;

            if (this.UserIsLoggedIn) this.Username = $"{CurrentUser.Username} ({CurrentUser.Role})";
            else this.Username = "";
        }
    }
}
