using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Calories.App.Views.LoginPage;
using Calories.App.Views.MealsListPage;
using Calories.App.Models;
using Calories.App.Entities.Users;
using Calories.App.Views.UsersListPage;
using Calories.App.Views.SettingsPage;

namespace Calories.App
{
    public partial class AppShell : Shell
    {
        private AppShellViewModel _vm => (AppShellViewModel)this.BindingContext;

        private readonly MenuItem LogoutButton;
        private readonly FlyoutItem LoginButton;
        private readonly FlyoutItem MealsButton;
        private readonly FlyoutItem UsersButton;
        private readonly FlyoutItem UserSettingsButton;

        public AppShell()
        {
            InitializeComponent();

            // Login
            this.LoginButton = new FlyoutItem()
            {
                Title = "Login",
                Icon = ImageSource.FromFile("login.png"),
            };
            this.LoginButton.Items.Add(new ShellContent() { ContentTemplate = new DataTemplate(() => new LoginPage()) });

            // Meals
            this.MealsButton = new FlyoutItem()
            {
                Title = "Meals",
                Icon = ImageSource.FromFile("meals_list.png"),
            };
            this.MealsButton.Items.Add(new ShellContent() { ContentTemplate = new DataTemplate(() => new MealsListPage()) });

            // Users
            this.UsersButton = new FlyoutItem()
            {
                Title = "Users",
                Icon = ImageSource.FromFile("users.png"),
            };
            this.UsersButton.Items.Add(new ShellContent() { ContentTemplate = new DataTemplate(() => new UsersListPage()) });

            // User Settings
            this.UserSettingsButton = new FlyoutItem()
            {
                Title = "User Settings",
                Icon = ImageSource.FromFile("settings.png"),
            };
            this.UserSettingsButton.Items.Add(new ShellContent() { ContentTemplate = new DataTemplate(() => new SettingsPage()) });

            // Logout
            this.LogoutButton = new MenuItem()
            {
                Text = "Logout",
                IconImageSource = ImageSource.FromFile("logout.png"),
                Command = _vm.LogoutCommand
            };

            this.Update();

            this._vm.PropertyChanged += (s, a) => this.Update();
        }

        private void Update()
        {
            if (_vm.UserIsLoggedIn)
            {
                // Order is important here.

                // Add meals (admin and members only)
                if ((_vm.CurrentUser.Role == UserRoles.Admin || _vm.CurrentUser.Role == UserRoles.Member) &&
                    !this.Items.Contains(this.MealsButton))
                {
                    this.Items.Add(this.MealsButton);
                }

                // Add users (admin and managers only)
                if ((_vm.CurrentUser.Role == UserRoles.Admin || _vm.CurrentUser.Role == UserRoles.Manager) &&
                    !this.Items.Contains(this.UsersButton))
                {
                    this.Items.Add(this.UsersButton);
                }

                // Add user settings (members only)
                if ((_vm.CurrentUser.Role == UserRoles.Member) &&
                    !this.Items.Contains(this.UserSettingsButton))
                {
                    this.Items.Add(this.UserSettingsButton);
                }

                // Add logout
                if (ShellItemFor(this.LogoutButton) == null) this.Items.Add(this.LogoutButton);

                // Remove login
                this.Items.Remove(this.LoginButton);
            }
            else
            {
                // Add login
                if (!this.Items.Contains(this.LoginButton)) this.Items.Add(this.LoginButton);

                // Remove meals
                this.Items.Remove(this.MealsButton);

                // Remove users
                this.Items.Remove(this.UsersButton);

                // Remove user settings
                this.Items.Remove(this.UserSettingsButton);

                // Remove logout
                if (ShellItemFor(this.LogoutButton) is ShellItem logoutButton) this.Items.Remove(logoutButton);
            }
        }

        private ShellItem ShellItemFor(MenuItem item)
        {
            return this.Items.FirstOrDefault(shellItem => shellItem.Title == item.Text);
        }
    }
}
