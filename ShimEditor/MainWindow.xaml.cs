using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ShimEditor
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ShimFile _loadedShim;

        public ShimFile LoadedShimFile
        {
            get { return _loadedShim; }
            set { _loadedShim = value; NotifyPropertyChanged(); }
        }

        private ShimEntry _loadedEntry;

        public ShimEntry SelectedEntry
        {
            get { return _loadedEntry; }
            set { _loadedEntry = value; NotifyPropertyChanged(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RepShim_Click(object sender, RoutedEventArgs e)
        {
            var path = IO.OpenFileDialog();
            if (!string.IsNullOrEmpty(path))
            {
                ShimEntry en = new ShimEntry(path, SelectedEntry.Name);
                LoadedShimFile?.ReplaceEntry(SelectedEntry, en);
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var path = IO.OpenFileDialog();
            if (!string.IsNullOrEmpty(path))
            { 
                LoadedShimFile = new ShimFile(path);
                this.Title = $"Shim Editor [{LoadedShimFile.FileName}]";
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var path = IO.SaveFileDialog();
            if (!string.IsNullOrEmpty(path))
                LoadedShimFile?.Save(path);
        }

        private void ExtractEntry_Click(object sender, RoutedEventArgs e)
        {
            var path = IO.SaveFileDialog(SelectedEntry.SafeFileName);
            if (!string.IsNullOrEmpty(path))
                LoadedShimFile?.Extract(SelectedEntry, path);
        }

        private void DeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            LoadedShimFile?.DeleteEntry(SelectedEntry);
        }

        private void btnAddShimEntry_Click(object sender, RoutedEventArgs e)
        {
            var path = IO.OpenFileDialog();
            if (!string.IsNullOrEmpty(path))
            {
                ShimEntry entry = new ShimEntry(path);
                LoadedShimFile?.AddEntry(entry);
            }
        }

        private void btnReplaceBaseFile_Click(object sender, RoutedEventArgs e)
        {
            var path = IO.OpenFileDialog();
            if (!string.IsNullOrEmpty(path))
            {
                LoadedShimFile?.ReplaceBase(path);
            }
        }
    }
}
