using Calories.App.Entities.Users;
using Calories.App.Managers;
using Calories.App.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Calories.App.Views.UserForm
{
    public class UserFormViewModel : PropertyChangedNotifier
    {

        public string Username { get; set; }

        // For user role picker
        public bool IsMember { get; set; }
        public bool IsManager { get; set; }
        public bool IsAdmin { get; set; }
        public bool ShowUserRolePicker { get; set; }

        public ICommand SaveUserCommand { get; }

        private AppModel AppModel => Injector.Get<AppModel>();
        private AppManager AppManager => Injector.Get<AppManager>();
        private UsersManager UsersManager => Injector.Get<UsersManager>();

        public UserFormViewModel(User user)
        {
            this.Username = user.Username;

            this.IsMember = user.Role == UserRoles.Member;
            this.IsManager = user.Role == UserRoles.Manager;
            this.IsAdmin = user.Role == UserRoles.Admin;

            this.ShowUserRolePicker = AppModel.CurrentUser.Role == UserRoles.Admin;

            this.SaveUserCommand = AppManager.SafeCommand(async () =>
            {
                user.Username = this.Username;
                user.Role = IsMember ? UserRoles.Member :
                            IsManager ? UserRoles.Manager :
                            UserRoles.Admin;

                await UsersManager.Commit(user);

                await Shell.Current.Navigation.PopModalAsync();
            });
        }
    }
}
