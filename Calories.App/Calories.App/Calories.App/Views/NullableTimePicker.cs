using System;
using Xamarin.Forms;
using PropertyChanged;

namespace Calories.App.Views
{
    public class NullableTimePicker: TimePicker
    {
        private string _format = null;
        public static readonly BindableProperty NullableTimeProperty = BindableProperty.Create(
            nameof(NullableTime),
            typeof(TimeSpan?),
            typeof(NullableTimePicker),
            null,
            BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is NullableTimePicker _this)
                    _this.NullableTime = (TimeSpan?)newValue;
            }
        );

        [DoNotNotify]
        public TimeSpan? NullableTime
        {
            get { return (TimeSpan?)GetValue(NullableTimeProperty); }
            set 
            { 
                this.SetValue(NullableTimeProperty, value); 
                this.UpdateDate(); 
                this.OnPropertyChanged();
            }
        }

        private void UpdateDate()
        {
            if (NullableTime.HasValue) { if (null != _format) Format = _format; Time = NullableTime.Value; }
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
            if (propertyName == "Time") NullableTime = Time;
        }
    }
}
