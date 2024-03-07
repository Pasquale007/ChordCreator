using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace ChordCreater.Components {
    /// <summary>
    /// Interaktionslogik für UserText.xaml
    /// </summary>
    public partial class UserText : UserControl {

        public UserText() {
            InitializeComponent();
            TextLine.NewLine += (s, ev) => CreateNweLine(s);
        }

        private void CreateNweLine(object numberObj) {
            int number = Convert.ToInt32(numberObj);
            int maxRows = LineContainer.Children.Count;
            for (int i = 1; i <= 2; i++) {
                if (number == maxRows) {
                    Line newLine = new Line();
                    newLine.NewLine += (s, ev) => CreateNweLine(s);
                    LineContainer.Children.Add(newLine);
                    newLine.Focus();
                }
            }
        }

        private void Export(object sender, RoutedEventArgs e) {
            StringBuilder sb = new StringBuilder();
            foreach (Line line in LineContainer.Children) {
                sb.AppendLine(line.Export());
            }
            ExportWindow exportWindow = new ExportWindow(sb.ToString());
            exportWindow.Show();
        }
        private void Import(object sender, RoutedEventArgs e) {
            List<List<string>> lines = new List<List<string>>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                string filePath = openFileDialog.FileName;

                string fileContent = File.ReadAllText(filePath);
                string pattern = @"\\songsection\{(.*?)\}";
                Match match = Regex.Match(fileContent, pattern);
                if (match.Success) {
                    string songTitle = match.Groups[1].Value;
                    SharedValues.InstanceOf.setTitle(songTitle);
                }
                int beginSongIndex = fileContent.IndexOf("\\beginsong{}");
                int endSongIndex = fileContent.IndexOf("\\endsong", beginSongIndex);

                if (beginSongIndex != -1 && endSongIndex != -1) {
                    string songSection = fileContent.Substring(beginSongIndex, endSongIndex - beginSongIndex);
                    using (StringReader reader = new StringReader(songSection)) {
                        string line;
                        while ((line = reader.ReadLine()) != null) {
                            if (line.Contains("\\beginvers") || line.Contains("\\endvers") || line.Length == 0 || line.Contains("\\beginsong{}")) {
                                continue;
                            }
                            string text = "";
                            string chord = "";
                            bool isChord = false;
                            foreach (char character in line) {
                                // skip \ symbol
                                if (character == '\\') {
                                    continue;
                                    // if [ expect the next character to be a chord
                                } else if (character == '[') {
                                    isChord = true;
                                    continue;
                                    // if ] expect that all symbols in the chord has been placed
                                } else if (character == ']') {
                                    isChord = false;
                                    continue;
                                    // if it is a normal character 
                                } else {
                                    // check if isChord flag is set if so -> append to chord and not to text
                                    if (isChord) {
                                        chord += character;
                                    } else {
                                        chord += " ";
                                        text += character;
                                    }
                                }
                            }
                            lines.Add(new List<string> { chord, text });
                        }
                    }
                } else {
                    Console.WriteLine("Das Muster '\\beginsong{}' oder '\\endsong' wurde nicht gefunden. Bitte stelle sicher, dass du die .tex Datei öffnest.");
                }
            }
            LineContainer.Children.Clear();
            foreach (List<string> line in lines) {
                LineContainer.Children.Add(new Line(line[0]));
                LineContainer.Children.Add(new Line(line[1]));
            }
        }

    }
}