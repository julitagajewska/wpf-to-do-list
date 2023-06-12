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

namespace ToDoListApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
                usernamePlaceholder.Visibility = Visibility.Visible;
            else
                usernamePlaceholder.Visibility = Visibility.Hidden;
        }

        private void txtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            usernamePlaceholder.Visibility = Visibility.Hidden;
        }

        private void txtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
                usernamePlaceholder.Visibility = Visibility.Visible;
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
                emailPlaceholder.Visibility = Visibility.Visible;
            else
                emailPlaceholder.Visibility = Visibility.Hidden;
        }

        private void txtEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            emailPlaceholder.Visibility = Visibility.Hidden;
        }

        private void txtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
                emailPlaceholder.Visibility = Visibility.Visible;
        }
    }
}
