using w4b.carnac.UI;

namespace Carnac.UI {
    public partial class PreferencesView {
        public PreferencesView(PreferencesViewModel viewModel) {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}