using Xamarin.Forms;

namespace Calories.App.Util
{
    /// <summary>Effect that removes default padding on buttons (android only).</summary>
    public class NoDefaultPadding : RoutingEffect
    {
        public NoDefaultPadding() : base("Calories.App.NoDefaultPadding") { }
    }
}
