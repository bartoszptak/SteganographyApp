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


        private Bitmap image_file = null;
        private Tuple<byte[],string> final_result = null;

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

        private async void Decrypt_button_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (image_file == null)
            {
                Raise_error("Open image file, please!");
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

            


            await Task.Run(() =>
            {
                final_result = engine.Decrypt_bytes();
            });

            if (final_result == null)
            {
                Raise_error("No message decrypted! Check source file, orientation and stop words.");
                return;
            }

            save_button.Visibility = Visibility.Visible;
            Raise_success("Text decrypted to textbox.");
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

        private void Save(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(final_result.Item1, 0, final_result.Item1.Length);               
            }
        }

        private void Save_button_Click(object sender, RoutedEventArgs e)
        {
            LSB_method.LSB_method engine = new LSB_method.LSB_method();

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "File |*" + final_result.Item2;

            var result = dlg.ShowDialog();

            if (result == true)
            {
                Save(dlg.FileName);

                Raise_success("Save successful!");
            }
            else
            {
                Raise_error("Save error!");
            }


        }

    }
}
