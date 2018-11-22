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
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace SteganographyApp
{
    /// <summary>
    /// Interaction logic for Binary_decrypt_user_control.xaml
    /// </summary>
    public partial class Binary_decrypt_user_control : UserControl
    {
        public Binary_decrypt_user_control()
        {
            InitializeComponent();
        }


        private void Load_image(string path)
        {
            image_panel.Source = new BitmapImage(new Uri(path));

        }


        private void Image_drop_panel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                Load_image(files[0]);
            }
        }

       

        private void Image_drop_panel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "Image files|*.jpeg;*.png;*.jpg;*.gif;*.png";

            var result = dlg.ShowDialog();

            if (result == true)
            {
                Load_image(dlg.FileName);
            }
        }

    }
}
