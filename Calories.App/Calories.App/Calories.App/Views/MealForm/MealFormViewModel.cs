using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;

using Xamarin.Forms;

using Calories.App.Models;
using Calories.App.Managers;
using Calories.App.Entities;
using Calories.App.Entities.Users;

namespace Calories.App.Views.MealForm
{
    public class MealFormViewModel : PropertyChangedNotifier
    {
        public bool CanSave { get; set; } = true;

        public bool UserPickerIsVisible { get; set; }
        public List<string> MemberUsernames { get; set; }
        public int MealUserIndex { get; set; }

        public DateTime MealDate { get; set; }

        public TimeSpan MealTime { get; set; }
        public string MealName { get; set; }
        public int MealCalories { get; set; }

        public ICommand SaveMealCommand { get; }

        public Meal Meal { get; }

        private AppModel AppModel => Injector.Get<AppModel>();
        private AppManager AppManager => Injector.Get<AppManager>();
        private MealsManager MealsManager => Injector.Get<MealsManager>();

        public MealFormViewModel(Meal meal)
        {
            this.Meal = meal;

            this.MealDate = meal.Time.Date;
            this.MealTime = meal.Time.TimeOfDay;
            this.MealName = meal.Name;
            this.MealCalories = meal.Calories;

            this.UserPickerIsVisible = AppModel.CurrentUser.Role == UserRoles.Admin;
            
            if (AppModel.AllUsers is List<User> users)
            {
                this.MemberUsernames = users
                    .Where(user => user.Role == UserRoles.Member)
                    .Select(user => user.Username)
                    .ToList();

                if (users.Find(u => u.Id == meal.UserId) is User mealUser)
                {
                    this.MealUserIndex = this.MemberUsernames.IndexOf(mealUser.Username);
                }
            }

            this.SaveMealCommand = AppManager.SafeCommand(async () =>
            {
                try
                {
                    var currentUser = AppModel.CurrentUser;

                    this.CanSave = false;
                    this.Meal.Time = this.MealDate.Add(this.MealTime);
                    this.Meal.Name = this.MealName;
                    this.Meal.Calories = this.MealCalories;

                    if (meal.UserId == null && currentUser.Role == UserRoles.Member)
                    {
                        meal.UserId = AppModel.CurrentUser.Id;
                    } 
                    else if (currentUser.Role == UserRoles.Admin && AppModel.AllUsers is List<User> _users)
                    {
                        var mealUser = _users.Find(u => u.Username == this.MemberUsernames[MealUserIndex]);

                        this.Meal.UserId = mealUser.Id;
                    }

                    await MealsManager.Commit(this.Meal);

                    await Shell.Current.Navigation.PopModalAsync();
                }
                finally
                {
                    this.CanSave = true;
                }
            });
        }
    }
}
