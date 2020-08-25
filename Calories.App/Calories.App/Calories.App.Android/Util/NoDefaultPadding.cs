using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("Calories.App")]
[assembly: ExportEffect(typeof(Calories.App.Droid.Util.NoDefaultPadding), "NoDefaultPadding")]
namespace Calories.App.Droid.Util
{
    public class NoDefaultPadding : PlatformEffect
    {
        /// <summary>
        /// Initializer to avoid linking out.
        /// </summary>
        public static void Init() { }

        /// <summary>
        /// Initializes a new instance of the class.
        /// Empty constructor required for the odd Xamarin.Forms reflection constructor search
        /// </summary>
        public NoDefaultPadding() { }

        protected override void OnAttached()
        {
            this.Control?.SetPadding(0, 0, 0, 0);

            if (this.Control is Android.Widget.Button button)
            {
                // https://stackoverflow.com/questions/16394252/how-do-i-reduce-the-inner-padding-around-the-text-within-an-android-button-objec
                button.SetMinimumWidth(0);
                button.SetMinWidth(0);

                button.SetMinimumHeight(0);
                button.SetMinHeight(0);
            }
        }

        protected override void OnDetached() { }
    }
}