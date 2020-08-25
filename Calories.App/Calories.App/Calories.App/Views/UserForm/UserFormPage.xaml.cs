using Calories.App.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Calories.App.Views.UserForm
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserFormPage : ContentPage
    {
        public UserFormPage(User user)
        {
            InitializeComponent();

            this.BindingContext = new UserFormViewModel(user);
        }
    }
}