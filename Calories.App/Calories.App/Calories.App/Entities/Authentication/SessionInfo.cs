using Calories.App.Entities.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calories.App.Entities.Authentication
{
    public class SessionInfo
    {
        public string Uuid { get; set; }

        public User User { get; set; }

        public DateTime ExpirationTime { get; set; }

        public string RawSessionAccessToken { get; set; }

        public bool IsExpired => DateTime.Now > this.ExpirationTime;
    }
}
