using System;
using System.Text;
using System.Windows.Controls;

namespace ChordCreater.Components {

    internal class MyTabItem : TabItem {
        protected ScrollViewer scroller = new ScrollViewer();
        protected StackPanel stack = new StackPanel();
        protected TabControl SongStructure;

        public MyTabItem(TabControl songstructure) {
            scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroller.Content = stack;
            this.Content = scroller;
            SongStructure = songstructure;
        }

        public void AddLinesToTab() {
            Line chordLine = new Line();
            Line textLine = new Line();
            textLine.NewLine += (s, ev) => CreateNewLine(s);
            stack.Children.Add(chordLine);
            stack.Children.Add(textLine);
        }

        public void CreateNewLine(object numberObj) {
            int number = Convert.ToInt32(numberObj);
            int maxRows = stack.Children.Count;
            for (int i = 1; i <= 2; i++) {
                Line newLine = new Line();
                newLine.NewLine += (s, ev) => CreateNewLine(s);
                stack.Children.Add(newLine);
                newLine.Focus();
            }
        }

        public string Export() {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Header + UserText.STRUCTURE_ELEMENT);
            foreach (Line line in stack.Children) {
                sb.AppendLine(line.Export());
            }
            return sb.ToString();
        }
    }
}
