using System;
using System.Text;
using System.Collections.Generic;

namespace Calories.App.Entities.Users
{
    public class User
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public UserRoles Role { get; set; } = UserRoles.Member;

        public UserSettings Settings { get; set; }
    }
}
