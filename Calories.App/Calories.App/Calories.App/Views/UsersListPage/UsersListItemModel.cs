using System;
using System.Text;
using System.Windows.Input;
using System.Collections.Generic;

using Xamarin.Forms;

using Calories.App.Models;
using Calories.App.Managers;
using Calories.App.Views.UserForm;
using Calories.App.Entities.Users;

namespace Calories.App.Views.UsersListPage
{
    /// <summary>View model for each row of the users list.</summary>
    public class UsersListItemModel : PropertyChangedNotifier
    {
        public string Username { get; }
        public string UserRole { get; }

        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }

        private AppModel AppModel => Injector.Get<AppModel>();
        private AppManager AppManager => Injector.Get<AppManager>();
        private UsersManager UsersManager => Injector.Get<UsersManager>();

        public UsersListItemModel(User user, Action resetSwipe)
        {
            this.Username = user.Username;

            if (AppModel.CurrentUser.Id == user.Id) this.Username += " (you)";

            this.UserRole = user.Role.ToString();

            this.DeleteUserCommand = AppManager.SafeCommand(() => UsersManager.DeleteUser(user));

            this.EditUserCommand = AppManager.SafeCommand(async () =>
            {
                resetSwipe();
                await Shell.Current.Navigation.PushModalAsync(new UserFormPage(user));
            });
        }
    }
}
