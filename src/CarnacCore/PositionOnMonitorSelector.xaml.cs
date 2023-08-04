using Carnac.Logic.Native;
using Carnac.UI;
using System.Windows;
using System.Windows.Controls;

namespace Carnac {
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