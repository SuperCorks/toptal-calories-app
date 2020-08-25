using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Calories.App.Views
{
    public class NullableDatePicker: DatePicker
    {
        private string _format = null;
        public static readonly BindableProperty NullableDateProperty = BindableProperty.Create(
            nameof(NullableDate),
            typeof(DateTime?),
            typeof(NullableDatePicker),
            null,
            BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is NullableDatePicker _this)
                    _this.NullableDate = (DateTime?)newValue;
            }
        );

        [DoNotNotify]
        public DateTime? NullableDate
        {
            get { return (DateTime?)GetValue(NullableDateProperty); }
            set 
            { 
                this.SetValue(NullableDateProperty, value); 
                this.UpdateDate();
                this.OnPropertyChanged();
            }
        }

        private void UpdateDate()
        {
            if (NullableDate.HasValue) { if (null != _format) Format = _format; Date = NullableDate.Value; }
            else { _format = Format; Format = "Pick ..."; }
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            UpdateDate();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "Date") NullableDate = Date;
        }
    }
}
