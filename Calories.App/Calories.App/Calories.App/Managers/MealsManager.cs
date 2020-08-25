using Calories.App.Entities;
using Calories.App.Models;
using Calories.App.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Calories.App.Managers
{
    [Injector.Singleton]
    public class MealsManager
    {
        private AppModel AppModel => Injector.Get<AppModel>();
        private MealService MealService => Injector.Get<MealService>();

        public async Task Commit(Meal meal)
        {
            var oldMeal = meal;
            var newMeal = await MealService.Commit(oldMeal);

            AppModel.Meals.Remove(oldMeal);
            AppModel.Meals.Add(newMeal);

            AppModel.RaiseMealsChanged();
        }

        public async Task DeleteMeal(Meal meal)
        {
            if (meal.Id is string mealId) await MealService.Delete(mealId);

            AppModel.Meals.Remove(meal);
            AppModel.RaiseMealsChanged();
        }
    }
}
