using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Calories.App.Models
{
    /// <summary>
    /// Base class for classes that want to implement the <see cref="INotifyPropertyChanged"/> interface. Also contains
    /// code samples, instructions and advices for developers.
    /// </summary>
    /// 
    /// <remarks>
    /// The <see cref="INotifyPropertyChanged"/> interface is a core concept used by Xamarin Forms to bind view
    /// attributes to class properties but also very useful when building reactive apps.
    /// 
    /// - FODY -
    /// 
    /// If you want the property changed event to be raised automatically, add/install the Fody nuget package 
    /// (PropertyChanged.Fody) in your project's dependencies. Properties without a body in their setter will raise 
    /// the PropertyChanged event automatically (don't forget to add the FodyWeavers.xml file in your project's root 
    /// directory. You can just copy the one from this project.). It's important to understand that properties with a 
    /// setter that has a body (i.e. properties that have code written in the setter) will have to raise the 
    /// PropertyChanged event themselves. A helper method is provided in this class (see examples).
    /// 
    /// Fody useful attributes: [DoNotNotify], [DoNotCheckEquality], [AlsoNotifyFor(nameof(YourOtherProperty))]
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// 
    /// public class MyModel : PropertyChangeNotifier {
    /// 
    ///     public bool FiresAutomatically { get; set; }
    ///
    ///
    ///    public bool NeverFires;
    ///
    ///
    ///    public bool FiresManually
    ///    {
    ///        get => this._firesManually;
    ///        set
    ///        {
    ///            this._firesManually = value;
    ///
    ///            if (some condition) this.OnPropertyChanged();
    ///        }
    ///    }
    ///    private bool _firesManually;
    ///
    ///
    ///    [DoNotNotify] // Do not notify automatically
    ///    public bool FiredFromSomewhereElse { get; set; }
    ///
    ///    public void FireFromSomewhereElse()
    ///    {
    ///        this.OnPropertyChanged(nameof(FiredFromSomewhereElse));
    ///    }
    /// }
    /// </code>
    /// </example>
    /// 
    /// <see cref="https://github.com/Fody/PropertyChanged"/>
    /// <see cref="https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx"/>
    public abstract class PropertyChangedNotifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
