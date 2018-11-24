using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for Text_encrypt_user_control.xaml
    /// </summary>
    public partial class Text_encrypt_user_control : UserControl
    {
        public Text_encrypt_user_control()
        {
            InitializeComponent();
        }

        private Bitmap image_file = null;
        private Bitmap image_result = null;

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
            dlg.Filter = "Image files|*.bmp;*.png;*.jpg;*.png";

            var result = dlg.ShowDialog();

            if (result == true)
            {
                Load_image(dlg.FileName);
            }
            else
            {
                image_file = null;
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
            else if(text_to_encrypt_textbox.Text.Length < 1)
            {
                Raise_error("Write text to encrypt, please!");
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
            if (text_to_encrypt_textbox.Text.Length * 8 > engine.Get_lenght_image()*3-8)
            {
                Raise_error("Message is to long!");
                return;
            }
            engine.Encrypt_text(text_to_encrypt_textbox.Text);

            image_result = engine.Save();
            save_button.Visibility = Visibility.Visible;

            // IF ALL OK
            Raise_success("Text encrypted in the image. Save it.");

        }

        private void orientation_togglebutton_Click(object sender, RoutedEventArgs e)
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

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Image files|*.bmp;*.png;*.jpg;*.png";

            var result = dlg.ShowDialog();

            if (result == true)
            {
                image_result.Save(dlg.FileName);
                Raise_success("Save successful!");
            }
            else
            {
                Raise_error("Save error!");
            }


        }
    }
}
