using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteganographyApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Load_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open_file_dialog = new OpenFileDialog
            {
                Filter = "Image files |*.png;*.jpg;*.bmp"
            };

            if (open_file_dialog.ShowDialog() == true)
            {
                image_box.Source = new BitmapImage(new Uri(open_file_dialog.FileName));
            }
        }
    }
}
