using Calories.App.Entities.Users;
using Calories.App.Managers;
using Calories.App.Models;
using Calories.App.Views.UserForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Calories.App.Views.UsersListPage
{
    public class UsersListPageModel : PropertyChangedNotifier
    {
        private readonly Action resetSwipe;

        public ICommand AddUserCommand { get; }
        public IEnumerable<UsersListItemModel> Users { get; set; }

        private AppModel AppModel => Injector.Get<AppModel>();
        private AppManager AppManager => Injector.Get<AppManager>();

        public UsersListPageModel(Action resetSwipe)
        {
            this.AddUserCommand = AppManager.SafeCommand(async () =>
            {
                resetSwipe();
                await Shell.Current.Navigation.PushModalAsync(new UserFormPage(new User()));
            });


            this.Update();

            AppModel.PropertyChanged += (s, a) => this.Update();
            this.resetSwipe = resetSwipe;
        }

        public void Update()
        {
            if (AppModel.AllUsers != null)
            {
                if (AppModel.CurrentUser?.Role == UserRoles.Admin)
                {
                    this.Users = AppModel.AllUsers
                        .OrderBy(user => user.Username)
                        .Select(user => new UsersListItemModel(user, resetSwipe));
                }
                else if (AppModel.CurrentUser?.Role == UserRoles.Manager)
                {
                    this.Users = AppModel.AllUsers
                        .Where(user => user.Role == UserRoles.Member)
                        .OrderBy(user => user.Username)
                        .Select(user => new UsersListItemModel(user, resetSwipe));
                }
            }
        }
    }
}
