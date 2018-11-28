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

        private Bitmap image_file = null;
        private Bitmap image_result = null;
        private byte[] file_to_encrypt = null;
        private string file_name = "";

        private void Load_image(string path)
        {
            image_panel.Source = new BitmapImage(new Uri(path));
            image_file = new Bitmap(path);
        }

        private void Raise_error(string message)
        {
            error_texbox.Text = message;
            error_texbox.Visibility = Visibility.Visible;
            error_texbox.Background = System.Windows.Media.Brushes.DarkRed;
        }

        private void Raise_success(string message)
        {
            error_texbox.Text = message;
            error_texbox.Visibility = Visibility.Visible;
            error_texbox.Background = System.Windows.Media.Brushes.DarkGreen;
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
            LSB_method.LSB_method engine = new LSB_method.LSB_method();
            dlg.Filter = engine.Get_formats();

            var result = dlg.ShowDialog();

            if (result == true)
            {
                Load_image(dlg.FileName);
            }
        }

        private void Load_file(string path)
        {
            using (BinaryReader b = new BinaryReader(
                    File.Open(path, FileMode.Open)))
            {
                int pos = 0;
                int length = (int)b.BaseStream.Length;
                byte[] res = new byte[length];
                while (pos < length)
                {
                    res[pos / sizeof(byte)] = b.ReadByte();
                    pos += sizeof(byte);
                }
                file_to_encrypt = res;
                file_name = path;

    }
}

        private void File_drop_panel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                Load_file(files[0]);
                file_name_texbox.Text = files[0];
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
                file_name_texbox.Text = dlg.FileName;
            }
        }

        private void Encrypt_button_Click(object sender, RoutedEventArgs e)
        {
            save_button.Visibility = Visibility.Hidden;
            if (image_file == null)
            {
                Raise_error("Open image file, please!");
                return;
            }
            else if (file_to_encrypt == null)
            {
                Raise_error("Open file to encrypt, please!");
                return;
            }
            else if (stop_words_textbox.Text.Length != 3)
            {
                Raise_error("Lenght of stop words must be equals 3 characters!");
                return;
            }


            Fall_error();
            var engine = new LSB_method.LSB_method(stop_words_textbox.Text, orientation_togglebutton.IsChecked.Value);

            engine.Load(image_file);
            if (file_to_encrypt.Length * 8 > engine.Get_lenght_image() * 3 - 8)
            {
                Raise_error("The file is too big!");
                return;
            }

            string[] ex = file_name.Split('.');
            loading_image.Visibility = Visibility.Visible;
            engine.Encrypt_bytes(file_to_encrypt, "."+ex[ex.Length-1]);

            image_result = engine.Get_image();
            save_button.Visibility = Visibility.Visible;
            loading_image.Visibility = Visibility.Hidden;

            Raise_success("File encrypted in the image. Save it.");
        }

        private void Orientation_togglebutton_Click(object sender, RoutedEventArgs e)
        {
            if (orientation_label != null && orientation_togglebutton != null)
            {
                if (orientation_togglebutton.IsChecked.Value)
                {
                    orientation_label.Content = "Horizontal";
                }
                else
                {
                    orientation_label.Content = "Vertical";
                }
            }
        }

        private void Save_button_Click(object sender, RoutedEventArgs e)
        {
            LSB_method.LSB_method engine = new LSB_method.LSB_method();

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = engine.Get_formats();

            var result = dlg.ShowDialog();

            if (result == true)
            {
                engine.Save_in_path(image_result, dlg.FileName);

                Raise_success("Save successful!");
            }
            else
            {
                Raise_error("Save error!");
            }


        }
    }
}
