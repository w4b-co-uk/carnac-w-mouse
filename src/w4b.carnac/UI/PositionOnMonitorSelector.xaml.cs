using Carnac.Logic.Native;
using System.Windows;
using System.Windows.Controls;
using w4b.carnac.UI;

namespace Carnac.UI {
    public partial class PositionOnMonitorSelector {
        public PositionOnMonitorSelector() {
            InitializeComponent();
        }

        private void RadioChecked(object sender, RoutedEventArgs e) {
            if (DataContext is PreferencesViewModel dc &&
                sender is RadioButton rb &&
                rb.Tag is DetailedScreen tag) {
                dc.SelectedScreen = tag;
            }
        }
    }
}