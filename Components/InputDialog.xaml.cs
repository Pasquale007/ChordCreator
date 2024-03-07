using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChordCreater {
    /// <summary>
    /// Interaktionslogik für InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window {
        public string Chord { get; private set; }
        public event EventHandler<string> ChordEntered;

        public InputDialog(string heading, string text) {
            InitializeComponent();
            Heading.Text = $"{heading}";
            Text.Text = $"{text}";
            ChordTextBox.Focus();
            ChordTextBox.PreviewKeyDown += (sender, e) => {
                if (e.Key == Key.Enter) {
                    OkButton_Click(sender, e);
                }
            };
        }

        private void OkButton_Click(object sender, RoutedEventArgs e) {
            Chord = ChordTextBox.Text;
            string chord = ChordTextBox.Text;
            ChordEntered?.Invoke(this, chord);
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }

    }
}
