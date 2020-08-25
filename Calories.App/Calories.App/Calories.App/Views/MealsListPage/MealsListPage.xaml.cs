using System;
using System.Collections;
using System.Globalization;
using Calories.App.Entities.Users;
using Calories.App.Models;
using Syncfusion.DataSource;
using Syncfusion.XForms.PopupLayout;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Calories.App.Views.MealsListPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MealsListPage : ContentPage
    {
        // https://help.syncfusion.com/xamarin/sfpopuplayout/styles
        private SfPopupLayout _filtersPopupLayout;

        public static GroupColorConverterClass GroupColorConverter { get; } = new GroupColorConverterClass();
        public static GroupHeaderFontConverterClass GroupHeaderFontConverter { get; } = new GroupHeaderFontConverterClass();

        public AppModel AppModel => Injector.Get<AppModel>();

        public MealsListPage()
        {
            InitializeComponent();

            this.BindingContext = new MealsListPageModel(
                showFiltersPopup: () =>
                {
                    this.SetupFiltersPopup();
                    this._filtersPopupLayout.Show();
                },
                resetSwipe: () => this.MealsList.ResetSwipe()
            );
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.MealsList.DataSource.GroupDescriptors.Clear();

            this.MealsList.DataSource.GroupDescriptors.Add(new GroupDescriptor()
            {
                PropertyName = nameof(MealListItemModel.MealDayText)
            });

            if (AppModel.CurrentUser.Role == UserRoles.Admin)
                this.MealsList.DataSource.GroupDescriptors.Add(new GroupDescriptor()
                {
                    PropertyName = nameof(MealListItemModel.Username)
                });
            

            if (this.BindingContext is MealsListPageModel vm) vm.OnAppearing();
        }

        private void SetupFiltersPopup()
        {
            this._filtersPopupLayout = new SfPopupLayout();
            this._filtersPopupLayout.Padding = new Thickness(20);

            var popupView = this._filtersPopupLayout.PopupView;
            popupView.HeaderTitle = " Filters";
            popupView.AcceptButtonText = "DONE";
            popupView.ContentTemplate = new DataTemplate(() => new FiltersForm.FiltersForm());
            popupView.HeightRequest = 300;
            popupView.WidthRequest = 360;

            popupView.PopupStyle.HeaderBackgroundColor = Color.FromHex("#1749D4");
            popupView.PopupStyle.HeaderTextColor = Color.White;
            popupView.PopupStyle.HeaderFontSize = 20;
            popupView.PopupStyle.HeaderTextAlignment = TextAlignment.Start;
            popupView.PopupStyle.AcceptButtonTextColor = Color.FromHex("#1749D4");
        }

        public class GroupColorConverterClass : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is IEnumerable items)
                {
                    foreach (var item in items)
                    {
                        if (item is MealListItemModel mealModel)
                        {
                            return mealModel.Color;
                        }
                    }
                }

                return Color.Blue;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public class GroupHeaderFontConverterClass : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is string groupingKey)
                {
                    if (groupingKey.Contains("@"))
                    {
                        return 14; // Username
                    }
                    else
                    {
                        return 20; // Date
                    }
                }

                return 20;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}