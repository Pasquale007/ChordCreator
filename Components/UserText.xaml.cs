using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChordCreater.Components {
    /// <summary>
    /// Interaktionslogik für UserText.xaml
    /// </summary>
    public partial class UserText : UserControl {
        private TabItem currentTabItem = null;
        private readonly string STRUCTURE_ELEMENT = "%";

        public UserText() {
            InitializeComponent();
        }

        private void CreateNewLine(object numberObj) {
            Console.WriteLine(currentTabItem.Header);
            ScrollViewer scroller = (ScrollViewer)currentTabItem.Content;
            StackPanel stack = (StackPanel)scroller.Content;
            int number = Convert.ToInt32(numberObj);
            int maxRows = stack.Children.Count;
            for (int i = 1; i <= 2; i++) {
                Line newLine = new Line();
                newLine.NewLine += (s, ev) => CreateNewLine(s);
                stack.Children.Add(newLine);
                newLine.Focus();
            }
        }

        private void Export(object sender, RoutedEventArgs e) {
            StringBuilder sb = new StringBuilder();
            foreach (TabItem item in SongStructure.Items) {
                if (item.Name == "PlusIcon") {
                    continue;
                }
                ScrollViewer scroller = (ScrollViewer)item.Content;
                StackPanel stack = (StackPanel)scroller.Content;
                sb.Append(item.Header + STRUCTURE_ELEMENT);
                foreach (Line line in stack.Children) {
                    sb.AppendLine(line.Export());
                }
                Console.WriteLine(sb.ToString());
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
                        TabItem newTab = CreateTab();
                        newTab.Header = line[1].Substring(0, line[1].Count() - 1);
                        SongStructure.Items.Add(newTab);
                        currentTabItem = newTab;
                    } else {
                        ScrollViewer scroller = (ScrollViewer)currentTabItem.Content;
                        StackPanel stack = (StackPanel)scroller.Content;
                        stack.Children.Add(new Line(line[0]));
                        Line textLine = new Line(line[1]);
                        textLine.NewLine += (s, ev) => CreateNewLine(s);
                        stack.Children.Add(textLine);
                    }
                }
            }
        }

        private void AddTab_Click(object sender, RoutedEventArgs e) {
            //set current tabItem to normal TabItem
            TabItem current = (TabItem)sender;
            current.Name = "";
            current.Header = "rename me";
            current.Style = null;
            current.MouseDoubleClick += Rename;
            ScrollViewer scroller = new ScrollViewer();
            scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            StackPanel stack = new StackPanel();
            scroller.Content = stack;
            current.Content = scroller;
            AddLinesToTab(current);

            //add plus TabItem
            Style plusIconStyle = new Style(typeof(TabItem));
            EventSetter eventSetter = new EventSetter(TabItem.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(AddTab_Click));
            plusIconStyle.Setters.Add(eventSetter);

            TabItem newTab = CreateTab();
            newTab.Header = "+";
            newTab.Name = "PlusIcon";
            newTab.Style = plusIconStyle;


            SongStructure.Items.Add(newTab);
            e.Handled = true;

        }

        private void Rename(object sender, MouseButtonEventArgs e) {
            TabItem selectedItem = (TabItem)sender;
            if ("PlusIcon" == selectedItem.Name) {
                return;
            }
            TextBox newHeader = new TextBox();

            //TODO: quickfix for selection of header with double click -> evtl contextmenu
            if (selectedItem.Header is TextBox) {
                return;
            }
            newHeader.Text = (string)selectedItem.Header;
            newHeader.Focus();
            newHeader.SelectAll();
            newHeader.LostFocus += (s, ev) => { selectedItem.Header = newHeader.Text; };
            newHeader.KeyDown += (s, ev) => {
                if (ev.Key == Key.Enter) {
                    selectedItem.Header = newHeader.Text;
                }
            };
            selectedItem.Header = newHeader;
        }

        private TabItem CreateTab() {
            TabItem newTab = new TabItem();
            ScrollViewer scroller = new ScrollViewer();
            scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            StackPanel stack = new StackPanel();
            scroller.Content = stack;
            newTab.Content = scroller;
            newTab.MouseDoubleClick += Rename;
            return newTab;
        }

        private void AddLinesToTab(TabItem item) {
            ScrollViewer scroller = (ScrollViewer)item.Content;
            StackPanel stack = (StackPanel)scroller.Content;
            Line chordLine = new Line();
            Line textLine = new Line();
            textLine.NewLine += (s, ev) => CreateNewLine(s);
            stack.Children.Add(chordLine);
            stack.Children.Add(textLine);
        }

        private void ChangeTab(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count == 0) {
                return;
            }
            TabItem selectedTab = e.AddedItems[0] as TabItem;
            Console.WriteLine(selectedTab.Header);
            currentTabItem = selectedTab;
        }
    }
}