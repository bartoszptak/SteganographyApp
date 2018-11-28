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
            principal_grid.Children.Clear();
            principal_grid.Children.Add(new Main_user_control());
        }


        private void Move_cursor_menu(int index)
        {
            cursor_grid.Margin = new Thickness(0, (100 + (60 * index)), 0, 0);
        }



        private void Menu_list_view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = menu_list_view.SelectedIndex;
            Move_cursor_menu(index);

            switch (index)
            {
                case 0:
                    principal_grid.Children.Clear();
                    principal_grid.Children.Add(new Main_user_control());
                    break;

                case 1:
                    principal_grid.Children.Clear();
                    principal_grid.Children.Add(new Text_user_control());
                    break;

                case 2:
                    principal_grid.Children.Clear();
                    principal_grid.Children.Add(new Binary_user_control());
                    break;

                case 3:
                    principal_grid.Children.Clear();
                    principal_grid.Children.Add(new Info_user_control());
                    break;

                case 4:
                    Application.Current.Shutdown();
                    break;

                default:
                    break;
            }

        }

        private void Github_button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/bartoszptak");
        }
    }
}
