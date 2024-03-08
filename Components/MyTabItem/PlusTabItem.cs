using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace ChordCreater.Components {
    internal class PlusTabItem : MyTabItem {

        public PlusTabItem(TabControl songStructure) : base(songStructure) {
            this.SongStructure = songStructure;
            scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroller.Content = stack;

            Style plusIconStyle = new Style(typeof(TabItem));
            EventSetter eventSetter = new EventSetter(TabItem.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(AddTab_Click));
            plusIconStyle.Setters.Add(eventSetter);

            this.Header = "+";
            this.Name = "PlusIcon";
            this.Style = plusIconStyle;
        }

        private void AddTab_Click(object sender, RoutedEventArgs e) {
            //set current tabItem to normal TabItem
            this.Name = "";
            this.Header = "rename me";
            this.Style = null;
            this.ContextMenu = new MyContextMenu(SongStructure);
            this.Content = scroller;
            this.AddLinesToTab();
            this.Content = scroller;

            //add plus TabItem
            AddPlusTab();
        }

        private void AddPlusTab() {
            TabItem newTab = new PlusTabItem(SongStructure);
            SongStructure.Items.Add(newTab);
        }

    }
}
