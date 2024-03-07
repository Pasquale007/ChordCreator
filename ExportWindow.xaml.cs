using ChordCreater.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace ChordCreater {
    /// <summary>
    /// Interaktionslogik für ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window {
        private string exportedSong;

        public ExportWindow(string exportedSong) {
            InitializeComponent();
            this.exportedSong = exportedSong;
        }

        private string GenerateLatexFile(string songtitle, string sb, string type) {
            return $@"
\documentclass{{article}}
\usepackage[{type}]{{songs}}

\noversenumbers

\begin{{document}}

\songsection{{{songtitle}}}

\begin{{songs}}{{}}
\beginsong{{}}
\beginverse
{sb}
\endverse
\endsong
\end{{songs}}

\end{{document}}";
        }

        private void Export(object sender, RoutedEventArgs e) {
            List<string> versions = new List<string>();
            string songTitle = SharedValues.InstanceOf.getTitle();
            if (Chorded.IsChecked == true) {
                versions.Add("chorded");
            }
            if (Lyrics.IsChecked == true) {
                versions.Add("lyric");
            }
            foreach (string version in versions) {
                string filePath = Path.Combine(SharedValues.InstanceOf.getTargetFolder(), $"{songTitle}-{version}.tex");
                string result = GenerateLatexFile(songTitle, exportedSong, version);
                File.WriteAllText(filePath, result);
                Console.WriteLine("Textdatei erfolgreich exportiert: " + filePath);
                new CreatePdf(filePath);
            }
        }

        private void Cancel(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
