using System;
using System.Collections.Generic;
using System.Text;

namespace Calories.App.Entities
{
    public class Meal
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public int Calories { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
