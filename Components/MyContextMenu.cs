using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public class MyContextMenu : ContextMenu {
    TabControl tabControl;
    public MyContextMenu(TabControl tabControl) {
        InitializeMenuItems();
        this.tabControl = tabControl;
    }

    private void InitializeMenuItems() {
        MenuItem renameMenuItem = new MenuItem();
        renameMenuItem.Header = "Rename";
        renameMenuItem.Click += RenameMenuItem_Click;

        MenuItem deleteMenuItem = new MenuItem();
        deleteMenuItem.Header = "Delete";
        deleteMenuItem.Click += DeleteMenuItem_Click;

        Items.Add(renameMenuItem);
        Items.Add(deleteMenuItem);
    }

    private void RenameMenuItem_Click(object sender, RoutedEventArgs e) {
        MenuItem menuItem = (MenuItem)sender;
        ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
        FrameworkElement target = (FrameworkElement)contextMenu.PlacementTarget;
        TabItem selectedItem = (TabItem)target;
        if ("PlusIcon" == selectedItem.Name) {
            return;
        }
        TextBox newHeader = new TextBox();

        if (selectedItem.Header is TextBox) {
            return;
        }
        newHeader.Text = (string)selectedItem.Header;
        Keyboard.Focus(newHeader);
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

    private void DeleteMenuItem_Click(object sender, RoutedEventArgs e) {
        MenuItem menuItem = (MenuItem)sender;
        ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
        FrameworkElement target = (FrameworkElement)contextMenu.PlacementTarget;

        TabItem selectedTab = (TabItem)target;
        MessageBoxResult result = MessageBox.Show($"Möchten Sie den Abteil '{selectedTab.Header}' wirklich löschen?", "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes) {
            tabControl.Items.Remove(selectedTab);
        }
    }
}
