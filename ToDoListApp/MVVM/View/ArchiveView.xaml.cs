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
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.ViewModel;

namespace ToDoListApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for ArchiveView.xaml
    /// </summary>
    public partial class ArchiveView : UserControl
    {
        public ArchiveView()
        {
            InitializeComponent();
        }

        private void DataGridRow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            var viewModel = DataContext as ArchiveViewModel;
            var selectedTask = row.DataContext as MainTask;

            viewModel.ShowDetailsTaskViewCommand.Execute(selectedTask);
        }

        private void filteringInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var viewModel = DataContext as ArchiveViewModel;
            if (string.IsNullOrEmpty(filteringInput.Text))
            {
                placeholder.Visibility = Visibility.Visible;
                viewModel.LoadTasksCommand.Execute(null);
                textBox.Text = "";
            }
            else
            {
                categoryComboBox.SelectedItem = null;
                placeholder.Visibility = Visibility.Hidden;
                viewModel.FilterTasksCommand.Execute(textBox.Text);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ArchiveViewModel;
            viewModel.AllCategoriesButtonCommand.Execute(null);
            filteringInput.Text = "";
        }

        private void categoryComboBox_Selected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if(comboBox.SelectedItem != null)
            {
                filteringInput.Text = "";
            }
        }
    }
}
