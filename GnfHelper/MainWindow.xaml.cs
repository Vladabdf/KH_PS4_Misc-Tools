using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace GnfHelper
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<ImageConversionCommand> _images;

        public ObservableCollection<ImageConversionCommand> Images
        {
            get { return _images; }
            set { _images = value; NotifyPropertyChanged(); }
        }

        private ImageConversionCommand _selImage;

        public ImageConversionCommand SelectedImage
        {
            get { return _selImage; }
            set { _selImage = value; NotifyPropertyChanged(); }
        }


        public MainWindow()
        {
            InitializeComponent();
            Images = new ObservableCollection<ImageConversionCommand>();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnAddFiles_Click(object sender, RoutedEventArgs e)
        {
            var files = IO.OpenFileDialog();
            foreach (var file in files)
            {
                var img = new ImageConversionCommand(file);
                Images.Add(img);
            }
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists("output"))
                Directory.CreateDirectory("output");
            else
                ClearDirectory("output");

            if (!Directory.Exists("temp"))
                Directory.CreateDirectory("temp");
            else
                ClearDirectory("temp");

            foreach (var img in Images)
            {
                img.Save();
            }

        }

        private void ClearDirectory(string dir)
        {
            var info = new DirectoryInfo(dir);
            foreach (var file in info.GetFiles())
            {
                file.Delete();
            }
        }
    }
}
