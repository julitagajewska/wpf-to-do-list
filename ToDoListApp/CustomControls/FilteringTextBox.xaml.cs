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
using ToDoListApp.MVVM.ViewModel;

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
        private ICommand filterTasks;
        private string placeholderText;

        public ICommand FilterTasks
        {
            get { return filterTasks; }
            set
            {
                filterTasks = value;
            }
        }
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
            var textBox = sender as TextBox;
            var viewModel = DataContext as AllTasksViewModel;

            if (string.IsNullOrEmpty(filteringInput.Text))
            {
                placeholder.Visibility = Visibility.Visible;
                // viewModel.LoadTasksCommand.Execute(null);
                filterTasks.Execute(textBox.Text);
            }
            else
            {
                placeholder.Visibility = Visibility.Hidden;
                filterTasks.Execute(textBox.Text);
                // viewModel.FilterTracksCommand.Execute(textBox.Text);
            }
        }

        private void filteringInput_GotFocus(object sender, RoutedEventArgs e)
        {
            filteringInput.BorderThickness = new Thickness(0);
            inputBackground.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B448B"));
            placeholder.Visibility = Visibility.Hidden;
        }

        private void filteringInput_LostFocus(object sender, RoutedEventArgs e)
        {
            filteringInput.BorderThickness = new Thickness(0);
            inputBackground.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#343c72"));
            if (string.IsNullOrEmpty(filteringInput.Text))
                placeholder.Visibility = Visibility.Visible;
        }
    }
}
