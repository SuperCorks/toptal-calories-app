using System.Linq;
using Calories.App.Models;
using Calories.App.Entities;
using Calories.App.Entities.Users;
using Xamarin.Forms;
using System.Windows.Input;
using Calories.App.Managers;
using Calories.App.Views.MealForm;
using System;

namespace Calories.App.Views.MealsListPage
{
    public class MealListItemModel : PropertyChangedNotifier
    {
        public Meal Meal { get; }
        public string Username { get; }

        public string CaloriesLabelText { get; }
        public string MealTimeText { get; }
        public string MealTimePeriod { get; }

        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        // Used for grouping
        public string MealDayText { get; }

        public Color Color { get; }

        public (int, int, int) MealDay { get; }

        private AppModel AppModel => Injector.Get<AppModel>();
        private AppManager AppManager => Injector.Get<AppManager>();
        private MealsManager MealsManager => Injector.Get<MealsManager>();

        public MealListItemModel(Meal meal, Action resetSwipe)
        {
            this.Meal = meal;
            this.CaloriesLabelText = $"{this.Meal.Calories} Cal";
            this.MealTimeText = this.Meal.Time.ToString("h:mm");
            this.MealTimePeriod = this.Meal.Time.ToString("tt");

            if (AppModel.AllUsers != null && 
                AppModel.AllUsers.FirstOrDefault(u => u.Id == meal.UserId) is User user)
            {
                this.Username = user.Username;
            }

            this.MealDayText = this.Meal.Time.ToString("MMM d yyyy");


            this.MealDay = (meal.Time.Year, meal.Time.Month, meal.Time.Day);

            // black for admins, otherwise red/green 
            this.Color = AppModel.CurrentUser.Role == UserRoles.Admin ? Color.Black : 
                IsOverDailyLimit ? 
                Color.FromHex("#ED3D3D") : // red 
                Color.FromHex("#51B24E"); // green

            this.DeleteCommand = AppManager.SafeCommand(() => MealsManager.DeleteMeal(this.Meal));

            this.EditCommand = AppManager.SafeCommand(async () =>
            {
                resetSwipe();
                await Shell.Current.Navigation.PushModalAsync(new MealFormPage(this.Meal));
            });

        }

        /// <summary>Whether meals of the same day as this meal are above the user's daily threshold (in term of calories).</summary>
        public bool IsOverDailyLimit
        {
            get
            {
                var user = AppModel.CurrentUser;

                if (AppModel.Meals != null && user.Role == UserRoles.Member)
                {
                    var mealsForThisDay = AppModel.Meals.Where(meal => 
                        (meal.Time.Year, meal.Time.Month, meal.Time.Day).Equals(this.MealDay)
                    );

                    return mealsForThisDay.Select(meal => meal.Calories).Sum() >= user.Settings.DailyCalories;
                }

                return false;
            }
        }
    }
}
