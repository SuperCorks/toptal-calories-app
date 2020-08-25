using Calories.App.Models;
using System.Windows.Input;
using Calories.App.Services;
using Calories.App.Managers;
using Calories.App.Entities.Users;

namespace Calories.App.Views.SettingsPage
{
    public class SettingsPageViewModel : PropertyChangedNotifier
    {
        public int DailyCalories { get; set; }

        public bool CanSave { get; set; } = true;

        public ICommand SaveCommand { get; }

        private AppModel AppModel => Injector.Get<AppModel>();
        private AppManager AppManager => Injector.Get<AppManager>();
        private UserService UserService => Injector.Get<UserService>();


        public SettingsPageViewModel()
        {
            this.SaveCommand = AppManager.SafeCommand(async () =>
            {
                this.CanSave = false;

                try
                {
                    AppModel.CurrentUser.Settings.DailyCalories = this.DailyCalories;

                    AppModel.CurrentUser.Settings = await UserService.CommitSettings(
                        AppModel.CurrentUser.Id,
                        AppModel.CurrentUser.Settings
                    );
                }
                finally
                {
                    this.CanSave = true;
                }
            });

            this.Update();

            AppModel.PropertyChanged += (s, a) => this.Update();
        }

        private void Update()
        {
            var user = AppModel.CurrentUser;

            if (user != null && user.Role == UserRoles.Member)
            {
                this.DailyCalories = AppModel.CurrentUser.Settings.DailyCalories;
            }
            else this.DailyCalories = 0;
        }
    }
}
