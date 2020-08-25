using System;
using Calories.App.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Calories.App.Services
{
    [Injector.Singleton]
    public class MealService : BaseService
    {
        public Task<Meal[]> Search(string userId = null)
        {
             if (userId == null) return this.HttpGet<Meal[]>($"/meals");
            return this.HttpGet<Meal[]>($"/meals?userId={userId}");
        }

        public Task<Meal> Commit(Meal meal)
        {
            return this.HttpPost<Meal>("/meals", meal);
        }

        public Task Delete(string mealId)
        {
            return this.HttpDelete($"/meals/{mealId}");
        }
    }
}
