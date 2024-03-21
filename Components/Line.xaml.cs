using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChordCreater.Components {
    /// <summary>
    /// Interaktionslogik für Line.xaml
    /// </summary>
    public partial class Line : UserControl {
        private static SortedList<int, Line> lines = new SortedList<int, Line>();
        private Line accordingChordLine;
        static int lineID = 1;
        private SortedList<int, string> chordMapper = new SortedList<int, string>();
        private bool isLast = true;
        enum LineType {
            Chord,
            Text,
        };

        public event EventHandler NewLine;
        private LineType lineType;

        public string LineNumber {
            get { return Number.Text; }
            set { Number.Text = value; }
        }

        public string LineContent {
            get { return Text.Text; }
            set { Text.Text = value; }
        }

        public Line(string content) {
            InitializeComponent();
            Text.Text = content;
            LineNumber = lineID++.ToString();
            Number.Text = LineNumber;
            if (lineID % 2 == 0) {
                lineType = LineType.Chord;
                Text.IsReadOnly = true;
                Text.Background = new SolidColorBrush(Colors.LightGray);
                Text.Focusable = false;
                ClearButton.Visibility = System.Windows.Visibility.Visible;
                Number.Visibility = System.Windows.Visibility.Collapsed;
            } else {
                lineType = LineType.Text;
                accordingChordLine = lines[Int32.Parse(LineNumber) - 1];

                // get chords from accordingChordLine and map them to the correct index
                string chords = accordingChordLine.LineContent;
                Console.WriteLine(chords);
                int counter = 0;
                for (int i = 0; counter < chords.Length; i++) {
                    string chord = "";
                    int startIndex = counter;
                    while (chords.Length > i && chords.ElementAt(i) != ' ') {
                        chord += chords.ElementAt(i);
                        i++;
                    }

                    if (chord.Length > 0) {
                        AddChord(startIndex, chord);
                    }
                    counter++;
                }

                Text.MouseDoubleClick += Content_MouseDoubleClick;
                Text.PreviewKeyDown += Text_PreviewKeyDown;
            }
            lines.Add(Int32.Parse(LineNumber), this);
        }

        public Line() : this("") {

        }

        private void Content_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            TextBox textBox = (TextBox)sender;
            int caretIndex = textBox.CaretIndex;
            int startIndex = caretIndex;
            while (startIndex > 0 && !char.IsWhiteSpace(textBox.Text[startIndex - 1])) {
                startIndex--;
            }

            int endIndex = caretIndex;
            while (endIndex < textBox.Text.Length && !char.IsWhiteSpace(textBox.Text[endIndex])) {
                endIndex++;
            }

            string selectedWord = textBox.Text.Substring(startIndex, endIndex - startIndex); ;

            InputDialog inputDialog = new InputDialog("Akkord Einfügen", $"Gib den Akkord für das Wort: '{selectedWord}' ein.");
            inputDialog.ChordEntered += (s, chord) => {
                AddChord(startIndex, chord);
            };
            inputDialog.ShowDialog();
        }

        public string Export() {
            if (lineType == LineType.Chord) {
                return "";
            }
            string songtext = Text.Text;

            string combinedText = "";
            int textIndex = 0;
            for (int i = 0; i < songtext.Length; i++) {
                if (chordMapper.ContainsKey(i)) {
                    combinedText += $"\\[{chordMapper[i]}]";
                }
                combinedText += songtext[textIndex];

                textIndex++;
            }
            return combinedText;
        }

        private void Text_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (Text.Text.Length > 0 && isLast) {
                isLast = false;
                NewLine.Invoke(LineNumber, e);
            }
        }

        private void ClearLine(object sender, System.Windows.RoutedEventArgs e) {
            Text.Text = "";
        }

        private void AddChord(int startIndex, string chord) {
            if (chordMapper.ContainsKey(startIndex)) {
                chordMapper.Remove(startIndex);
            }

            chordMapper.Add(startIndex, chord);

            // Build the new text with spaces before the chord
            string newText = "";
            for (int i = 0; i < Text.Text.Length; i++) {
                if (chordMapper.ContainsKey(i)) {
                    newText += chordMapper[i];
                } else {
                    newText += " ";
                }
            }

            // Update the Text property of accordingChordLine with the new text
            accordingChordLine.Text.Text = newText;
        }
    }
}
