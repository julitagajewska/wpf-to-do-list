using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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
using ToDoListApp.MVVM.ViewModel;

namespace ToDoListApp.CustomControls
{
    /// <summary>
    /// Interaction logic for BindablePasswordBox.xaml
    /// </summary>
    public partial class BindablePasswordBox : UserControl
    {
        private string placeholderText;
        public string PlaceholderText
        {
            get { return placeholderText; }
            set
            {
                placeholderText = value;
                // OnPropertyChanged(nameof(PlaceholderText));
                placeholder.Text = placeholderText; // Zamienić na OnPropertyChanged()
            }
        }
        public static readonly DependencyProperty PasswordProperty = 
            DependencyProperty.Register("Password", typeof(SecureString), typeof(BindablePasswordBox));

        public SecureString Password
        {
            get
            {
                return (SecureString)GetValue(PasswordProperty);
            }

            set
            {
                SetValue(PasswordProperty, value);
            }
        }
        public BindablePasswordBox()
        {
            InitializeComponent();
            txtPassword.PasswordChanged += OnPasswordChanged;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = txtPassword.SecurePassword;
            var viewModel = (RegisterViewModel)DataContext;
            // viewModel.ValidatePasswordCommand.Execute(txtPassword.Password);
            if (Password.Length == 0)
            {
                placeholder.Visibility = Visibility.Visible;
            }
            else
            {
                placeholder.Visibility = Visibility.Hidden;
            }
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            placeholder.Visibility = Visibility.Hidden;
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            Password = txtPassword.SecurePassword;
            if (Password.Length == 0)
                placeholder.Visibility = Visibility.Visible;
        }
    }
}
