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

namespace ToDoListApp.CustomControls
{
    /// <summary>
    /// Interaction logic for FilteringTextBox.xaml
    /// </summary>
    public partial class FilteringTextBox : UserControl
    {
        public FilteringTextBox()
        {
            InitializeComponent();
        }

        private string placeholderText;

        public string PlaceholderText
        {
            get { return placeholderText; }
            set { 
                placeholderText = value;
                placeholder.Text = placeholderText; // Zamienić na OnPropertyChanged()
            }
        }


        private void filteringInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(filteringInput.Text))
                placeholder.Visibility = Visibility.Visible; 
            else
                placeholder.Visibility = Visibility.Hidden;
        }

        private void filteringInput_GotFocus(object sender, RoutedEventArgs e)
        {
            filteringInput.BorderThickness = new Thickness(0);
            inputBackground.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#343c72"));
            placeholder.Visibility = Visibility.Hidden;
        }

        private void filteringInput_LostFocus(object sender, RoutedEventArgs e)
        {
            filteringInput.BorderThickness = new Thickness(0);
            inputBackground.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B448B"));
            if (string.IsNullOrEmpty(filteringInput.Text))
                placeholder.Visibility = Visibility.Visible;
        }
    }
}
