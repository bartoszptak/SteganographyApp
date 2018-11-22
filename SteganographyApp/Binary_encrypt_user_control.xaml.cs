using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
    /// Interaction logic for Binary_encrypt_user_control.xaml
    /// </summary>
    public partial class Binary_encrypt_user_control : UserControl
    {
        public Binary_encrypt_user_control()
        {
            InitializeComponent();
        }

        private byte[] binary_file = null;
        private Bitmap image_file = null;

        private void Load_image(string path)
        {
            image_panel.Source = new BitmapImage(new Uri(path));
            image_file = new Bitmap(path);
        }

        private void Load_file(string path)
        {
            using (BinaryReader b = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                int pos = 0;
                int length = (int)b.BaseStream.Length;
                binary_file = new byte[length];
                while (pos < length)
                {
                    binary_file[pos / sizeof(byte)] = b.ReadByte();
                    pos += sizeof(byte);
                }
            }
            file_name_texbox.Text = System.IO.Path.GetFileName(path);

        }

        private void Raise_error(string message)
        {
            error_texbox.Text = message;
            error_texbox.Visibility = Visibility.Visible;
        }

        private void Fall_error()
        {
            error_texbox.Visibility = Visibility.Hidden;
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
            dlg.Filter = "Image files|*.jpeg;*.png;*.jpg;*.gif;*.png";

            var result = dlg.ShowDialog();

            if (result == true)
            {
                Load_image(dlg.FileName);
            }
        }

        private void File_drop_panel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                Load_file(files[0]);
            }
        }

        private void Open_file_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All files|*.*";

            var result = dlg.ShowDialog();

            if (result == true)
            {
                Load_file(dlg.FileName);
            }
        }



        private void encrypt_button_Click(object sender, RoutedEventArgs e)
        {
            int seed;

            if (!Int32.TryParse(seed_textbox.Text, out seed))
            {
                Raise_error("Seed must me integer number!");
                return;
            }
            else if(binary_file == null)
            {
                Raise_error("Open binary file, please!");
                return;
            }
            else if (image_file == null)
            {
                Raise_error("Open image file, please!");
                return;
            }



            // IF ALL OK
            Fall_error();

        }
    }
}
