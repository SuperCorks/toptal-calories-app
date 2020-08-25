using Calories.App.Entities.Users;
using Calories.App.Models;
using Calories.App.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Calories.App.Managers
{
    [Injector.Singleton(ActivationTypes.Eager)]
    internal class UsersManager
    {
        private AppModel AppModel => Injector.Get<AppModel>();
        private UserService UserService => Injector.Get<UserService>();

        public async Task Commit(User user)
        {
            var oldUser = user;
            var newUser = await UserService.Commit(oldUser);

            AppModel.AllUsers.Remove(oldUser);
            AppModel.AllUsers.Add(newUser);

            AppModel.RaiseMealsChanged();
        }

        public async Task DeleteUser(User user)
        {
            if (user.Id is string userId) await UserService.Delete(userId);

            AppModel.AllUsers.Remove(user);
            AppModel.RaiseUsersChanged();
        }
    }
}
