using CommunityToolkit.Mvvm.ComponentModel;

namespace Carnac.Logic {
    /// <summary>
    /// Base class for observable objects. Thin wrapper over CommunityToolkit.Mvvm's ObservableObject.
    /// </summary>
    public class NotifyPropertyChanged : ObservableObject {
    }
}