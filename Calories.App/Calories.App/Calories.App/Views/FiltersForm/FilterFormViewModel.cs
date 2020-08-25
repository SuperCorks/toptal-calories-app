using Calories.App.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Calories.App.Views.FiltersForm
{
    public class FilterFormViewModel : PropertyChangedNotifier
    {
        private AppModel AppModel => Injector.Get<AppModel>();

        public DateTime? FilterDateFrom 
        {
            get => AppModel.FilterDateFrom;
            set
            {
                AppModel.FilterDateFrom = value;
                RaisePropertyChangedEvent();
            }
        }

        public DateTime? FilterDateTo 
        {
            get => AppModel.FilterDateTo;
            set
            {
                AppModel.FilterDateTo = value;
                RaisePropertyChangedEvent();
            }
        }

        public TimeSpan? FilterTimeFrom 
        {
            get => AppModel.FilterTimeFrom;
            set
            {
                AppModel.FilterTimeFrom = value;
                RaisePropertyChangedEvent();
            }
        }

        public TimeSpan? FilterTimeTo 
        {
            get => AppModel.FilterTimeTo; 
            set
            {
                AppModel.FilterTimeTo = value;
                RaisePropertyChangedEvent();
            }
        }

        public ICommand ResetFilterDateFromCommand { get; }
        public ICommand ResetFilterDateToCommand { get; }
        public ICommand ResetFilterTimeFromCommand { get; }
        public ICommand ResetFilterTimeToCommand { get; }

        public FilterFormViewModel()
        {
            ResetFilterDateFromCommand = new Command(() => this.FilterDateFrom = null);
            ResetFilterDateToCommand = new Command(() => this.FilterDateTo = null);
            ResetFilterTimeFromCommand = new Command(() => this.FilterTimeFrom = null);
            ResetFilterTimeToCommand = new Command(() => this.FilterTimeTo = null);
        }
    }
}
