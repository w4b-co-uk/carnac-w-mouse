using System.Windows;
using System.Windows.Controls;

namespace w4b.carnac.UI {
    /// <summary>
    /// Interaction logic for ThreeColumnControl.xaml
    /// </summary>
    public partial class ThreeColumnControl: UserControl {
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(ThreeColumnControl), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty MainContentProperty =
            DependencyProperty.Register("MainContent", typeof(object), typeof(ThreeColumnControl), new PropertyMetadata(null));

        public static readonly DependencyProperty OptionalContentProperty =
            DependencyProperty.Register("OptionalContent", typeof(object), typeof(ThreeColumnControl), new PropertyMetadata(null));

        public string LabelText {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public object MainContent {
            get => GetValue(MainContentProperty);
            set => SetValue(MainContentProperty, value);
        }

        public object OptionalContent {
            get => GetValue(OptionalContentProperty);
            set => SetValue(OptionalContentProperty, value);
        }

        public ThreeColumnControl() {
            InitializeComponent();
        }
    }
}
