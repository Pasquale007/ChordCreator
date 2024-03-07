using System.Windows;

namespace ChordCreater.Components {
    public class SharedValues : DependencyObject {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SharedValues), new PropertyMetadata("Liedtitel"));


        private string targetFolder = "C:\\Users\\Pasca\\Desktop\\GenSongText";
        public static SharedValues InstanceOf { get; } = new SharedValues();

        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public void setTitle(string newTitle) {
            Title = newTitle;
        }

        public string getTitle() {
            return Title;
        }

        public string getTargetFolder() {
            return targetFolder;
        }

        private SharedValues() { }
    }
}
