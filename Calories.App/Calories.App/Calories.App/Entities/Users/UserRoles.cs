using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Calories.App.Entities.Users
{
    public enum UserRoles
    {
        [EnumMember(Value = "admin")]
        Admin,
        [EnumMember(Value = "member")]
        Member,
        [EnumMember(Value = "manager")]
        Manager
    }
}
