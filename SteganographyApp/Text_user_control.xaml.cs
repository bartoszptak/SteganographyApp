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
    /// Interaction logic for Text_user_control.xaml
    /// </summary>
    public partial class Text_user_control : UserControl
    {
        public Text_user_control()
        {
            InitializeComponent();
            principal_grid.Children.Clear();
            principal_grid.Children.Add(new Text_encrypt_user_control());
        }

        private void Button_click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);

            GridCursor.Margin = new Thickness(10 + (382 * index), 0, 0, -30);

            switch (index)
            {
                case 0:
                    principal_grid.Children.Clear();
                    principal_grid.Children.Add(new Text_encrypt_user_control());
                    break;
                case 1:
                    principal_grid.Children.Clear();
                    principal_grid.Children.Add(new Text_decrypt_user_control());
                    break;
            }

        }
    }
}
