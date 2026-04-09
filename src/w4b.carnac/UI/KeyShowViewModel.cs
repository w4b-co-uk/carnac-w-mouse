using Carnac.Logic;
using Carnac.Logic.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using Carnac.logic.Models;

namespace Carnac.UI {
    public partial class KeyShowViewModel : NotifyPropertyChanged {
        public KeyShowViewModel(PopupSettings popupSettings) {
            Messages = new ObservableCollection<Message>();
            Settings = popupSettings;
        }

        public ObservableCollection<Message> Messages { get; }

        [ObservableProperty]
        private PopupSettings settings;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CursorMargins))]
        private Point cursorPosition;

        public Thickness CursorMargins => new(CursorPosition.X - 10, CursorPosition.Y - 10, 0, 0);
    }
}
