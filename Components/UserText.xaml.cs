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
        public static readonly string STRUCTURE_ELEMENT = "%";

        private MyTabItem currentTabItem = null;

        public UserText() {
            InitializeComponent();
            AddPlusTab();
        }

        private void Export(object sender, RoutedEventArgs e) {
            StringBuilder sb = new StringBuilder();
            foreach (MyTabItem item in SongStructure.Items) {
                if (item.Name == "PlusIcon") {
                    continue;
                }
                sb.Append(item.Export());
            }

            ExportWindow exportWindow = new ExportWindow(sb.ToString());
            exportWindow.Show();
        }

        private void Import(object sender, RoutedEventArgs e) {
            List<List<string>> lines = new List<List<string>>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TeX Dateien (*.tex)|*.tex";
            if (openFileDialog.ShowDialog() == true) {
                string filePath = openFileDialog.FileName;
                if (!filePath.EndsWith(".tex", StringComparison.OrdinalIgnoreCase)) {
                    MessageBox.Show("Bitte wählen Sie eine .tex-Datei aus.");
                    return;
                }
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
                                // if [ expect the next character to be a chord
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
                SongStructure.Items.Clear();
                foreach (List<string> line in lines) {
                    if (line[1].Contains(STRUCTURE_ELEMENT)) {
                        MyTabItem newTab = new MyTabItem(SongStructure);
                        newTab.Header = line[1].Substring(0, line[1].Count() - 1);
                        SongStructure.Items.Add(newTab);
                        currentTabItem = newTab;
                    } else {
                        ScrollViewer scroller = (ScrollViewer)currentTabItem.Content;
                        StackPanel stack = (StackPanel)scroller.Content;
                        stack.Children.Add(new Line(line[0]));
                        Line textLine = new Line(line[1]);
                        textLine.NewLine += (s, ev) => currentTabItem.CreateNewLine(s);
                        stack.Children.Add(textLine);
                    }
                }
                AddPlusTab();
            }
        }

        private void AddPlusTab() {
            TabItem newTab = new PlusTabItem(SongStructure);
            SongStructure.Items.Add(newTab);
        }


        private void ChangeTab(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count == 0) {
                return;
            }
            MyTabItem selectedTab = e.AddedItems[0] as MyTabItem;
            currentTabItem = selectedTab;
        }
    }
}