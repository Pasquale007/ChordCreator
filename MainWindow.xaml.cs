using ChordCreater.Components;
using System.Windows;
using System.Windows.Controls;

namespace ChordCreater {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Liedtitel_TextChanged(object sender, TextChangedEventArgs e) {
            SharedValues.InstanceOf.setTitle(Liedtitel.Text);
        }
    }
}
