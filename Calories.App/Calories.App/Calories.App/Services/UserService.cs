using System.Threading.Tasks;
using System.Collections.Generic;
using Calories.App.Entities.Users;

namespace Calories.App.Services
{
    public class UserService : BaseService
    {
        public Task<User[]> Search(bool members, bool managers, bool admins)
        {
            var roles = new List<string>();
            if (members) roles.Add("member");
            if (managers) roles.Add("manager");
            if (admins) roles.Add("admin");
            return this.HttpGet<User[]>($"/users?roles={string.Join(",", roles)}");
        }

        public Task<User> Commit(User user)
        {
            return this.HttpPost<User>($"/users", user);
        }

        public Task<UserSettings> CommitSettings(string userId, UserSettings settings)
        {
            return this.HttpPost<UserSettings>($"/users/settings/{userId}", settings);
        }

        public Task Delete(string userId)
        {
            return this.HttpDelete($"/users/{userId}");
        }
    }
}
