using Calories.App.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Calories.App.Views.MealForm
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MealFormPage : ContentPage
    {
        private MealFormViewModel vm;

        public MealFormPage(Meal meal)
        {
            InitializeComponent();

            this.BindingContext = this.vm = new MealFormViewModel(meal);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Needs to be here or the picker doesn't have a default selected value
            this.UserPicker.SelectedIndex = vm.MealUserIndex;
        }
    }
}