using Calories.App.Entities;
using Calories.App.Entities.Users;
using Calories.App.Managers;
using Calories.App.Models;
using Calories.App.Views.MealForm;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Calories.App.Views.MealsListPage
{
    public class MealsListPageModel : PropertyChangedNotifier
    {
        private readonly Action resetSwipe;

        #region ViewComponents
        [DoNotCheckEquality]
        public IEnumerable<MealListItemModel> Meals { get; set; }

        public ICommand AddMealCommand { get; }
        public ICommand ShowFiltersPopupCommand { get; }
        #endregion ViewComponents

        private AppModel AppModel { get; } = Injector.Get<AppModel>();
        private AppManager AppManager { get; } = Injector.Get<AppManager>();

        public MealsListPageModel(Action showFiltersPopup, Action resetSwipe)
        {
            AppModel.PropertyChanged += (s, o) => this.Update();

            this.AddMealCommand = AppManager.SafeCommand(async ()  => {
                resetSwipe();
                await Shell.Current.Navigation.PushModalAsync(new MealFormPage(new Meal()));
            });

            this.ShowFiltersPopupCommand = new Command(showFiltersPopup);
            this.resetSwipe = resetSwipe;
        }

        public void OnAppearing()
        {
            this.Update();
        }

        private void Update()
        {
            // Set meals
            if (AppModel.Meals == null)
            {
                this.Meals = null;
            }
            else
            {
                var dateFrom = AppModel.FilterDateFrom;
                var dateTo = AppModel.FilterDateTo;
                var timeFrom = AppModel.FilterTimeFrom;
                var timeTo = AppModel.FilterTimeTo;

                this.Meals = AppModel.Meals
                    // Apply filters
                    .Where(meal => dateFrom == null || meal.Time.Date >= dateFrom)
                    .Where(meal => dateTo == null || meal.Time.Date <= dateTo)
                    .Where(meal => timeFrom == null || meal.Time.TimeOfDay >= timeFrom)
                    .Where(meal => timeTo == null || meal.Time.TimeOfDay <= timeTo)
                    .OrderByDescending(meal => meal.Time) // Order by time from latest to oldest
                    .Select(meal => new MealListItemModel(meal, resetSwipe));
            }
        }
    }
}
