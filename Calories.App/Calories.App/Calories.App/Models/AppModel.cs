using Calories.App.Entities;
using System.Collections.Generic;
using Calories.App.Entities.Users;
using System.Collections.ObjectModel;
using System;

namespace Calories.App.Models
{
    /// <summary>
    /// Singleton class that models the app's state. Get it through the <see cref="Injector"/>.
    /// </summary>
    [Injector.Singleton]
    public class AppModel : PropertyChangedNotifier
    {
        /// <summary>The current logged in user.</summary>
        public User CurrentUser { get; set; }

        /// <summary>The current logged in user.</summary>
        public List<User> AllUsers { get; set; } = new List<User>();

        /// <summary>
        /// Loaded meals. Either the current user's meals if the current user is a member or all 
        /// the meals if the current user is an admin.
        /// </summary>
        public List<Meal> Meals { get; set; } = new List<Meal>();

        /// <summary>Whether the app is executing the login process.</summary>
        public bool IsLoggingIn { get; set; }

        public DateTime? FilterDateTo { get; set; }
        public TimeSpan? FilterTimeTo { get; set; }
        public DateTime? FilterDateFrom { get; set; }
        public TimeSpan? FilterTimeFrom { get; set; }

        public void Reset()
        {
            this.Meals = null;
            this.AllUsers = null;
            this.CurrentUser = null;

            this.FilterDateTo = null;
            this.FilterTimeTo = null;
            this.FilterDateFrom = null;
            this.FilterTimeFrom = null;
        }

        /// <summary>Raises the <see cref="PropertyChanged"/> event for the <see cref="Meals"/> property.</summary>
        public void RaiseMealsChanged()
        {
            this.RaisePropertyChangedEvent(nameof(Meals));
        }

        /// <summary>Raises the <see cref="PropertyChanged"/> event for the <see cref="AllUsers"/> property.</summary>
        public void RaiseUsersChanged()
        {
            this.RaisePropertyChangedEvent(nameof(AllUsers));
        }
    }
}
