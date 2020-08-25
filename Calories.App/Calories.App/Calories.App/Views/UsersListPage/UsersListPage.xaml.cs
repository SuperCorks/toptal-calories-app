using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Calories.App.Views.UsersListPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersListPage : ContentPage
    {
        public UsersListPage()
        {
            InitializeComponent();

            this.BindingContext = new UsersListPageModel(() => this.UsersList.ResetSwipe());
        }
    }
}