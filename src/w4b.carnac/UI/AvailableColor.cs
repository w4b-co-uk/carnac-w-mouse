using Carnac.Logic;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace Carnac.UI {
    public partial class AvailableColor : NotifyPropertyChanged {
        public AvailableColor(string name, Color color) {
            Name = name;
            Brush = new SolidColorBrush(color);
        }

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private SolidColorBrush brush;
    }
}