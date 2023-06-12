using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.ViewModel;

namespace ToDoListApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for CreateTaskView.xaml
    /// </summary>
    public partial class CreateTaskView : UserControl
    {
        public CreateTaskView()
        {
            InitializeComponent();
        }
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            subtasksListBox.Items.Refresh();
        }
        private void CategoryListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            var selectedItems = listBox.SelectedItems.Cast<Category>().ToList();
            var viewModel = (CreateTaskViewModel)DataContext;
            viewModel.SelectedCategories = new ObservableCollection<Category>(selectedItems);
        }

        // ---- Name ---- //
        private void NameInputChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
                namePlaceholder.Visibility = Visibility.Visible;
            else
                namePlaceholder.Visibility = Visibility.Hidden;
        }

        private void NameInputGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            nameInputBackground.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B448B"));
            namePlaceholder.Visibility = Visibility.Hidden;
        }

        private void NameInputLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            nameInputBackground.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#343c72"));
            if (string.IsNullOrEmpty(textBox.Text))
                namePlaceholder.Visibility = Visibility.Visible;
        }

        // ---- Description ---- //
        private void DescriptionInputChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
                descriptionPlaceholder.Visibility = Visibility.Visible;
            else
                descriptionPlaceholder.Visibility = Visibility.Hidden;
        }

        private void DescriptionInputGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            descriptionInputBackground.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B448B"));
            descriptionPlaceholder.Visibility = Visibility.Hidden;
        }

        private void DescriptionInputLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            descriptionInputBackground.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#343c72"));
            if (string.IsNullOrEmpty(textBox.Text))
                descriptionPlaceholder.Visibility = Visibility.Visible;
        }

        // ---- Assign day ---- //
        private void AssignDayDatePickerGotFocus(object sender, RoutedEventArgs e)
        {
            var border = (Border)VisualTreeHelper.GetChild(assignDayDatePicker, 0);
            border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B448B"));
        }

        private void AssignDayDatePickerLostFocus(object sender, RoutedEventArgs e)
        {
            var border = (Border)VisualTreeHelper.GetChild(assignDayDatePicker, 0);
            border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#343c72"));
        }
        // ---- Deadline ---- //
        private void DeadlinePickerGotFocus(object sender, RoutedEventArgs e)
        {
            var border = (Border)VisualTreeHelper.GetChild(deadlineDatePicker, 0);
            border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B448B"));
        }

        private void DeadlinePickerLostFocus(object sender, RoutedEventArgs e)
        {
            var border = (Border)VisualTreeHelper.GetChild(deadlineDatePicker, 0);
            border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#343c72"));
        }
        // ---- Start date ---- //
        private void StartDatePickerGotFocus(object sender, RoutedEventArgs e)
        {
            var border = (Border)VisualTreeHelper.GetChild(startDatePicker, 0);
            border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B448B"));
        }

        private void StartDatePickerLostFocus(object sender, RoutedEventArgs e)
        {
            var border = (Border)VisualTreeHelper.GetChild(startDatePicker, 0);
            border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#343c72"));
        }
        // ---- End date ---- //
        private void EndDatePickerGotFocus(object sender, RoutedEventArgs e)
        {
            var border = (Border)VisualTreeHelper.GetChild(endDatePicker, 0);
            border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B448B"));
        }

        private void EndDatePickerLostFocus(object sender, RoutedEventArgs e)
        {
            var border = (Border)VisualTreeHelper.GetChild(endDatePicker, 0);
            border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#343c72"));
        }
        // ---- New category ---- //
        private void NewCategoryInputChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
                newCategoryPlaceholder.Visibility = Visibility.Visible;
            else
                newCategoryPlaceholder.Visibility = Visibility.Hidden;
        }
        private void NewCategoryInputGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            newCategoryInputBackground.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B448B"));
            newCategoryPlaceholder.Visibility = Visibility.Hidden;
        }

        private void NewCategoryInputLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            newCategoryInputBackground.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#343c72"));
            if (string.IsNullOrEmpty(textBox.Text))
                newCategoryPlaceholder.Visibility = Visibility.Visible;
        }

        // ---- New subtask ---- //
        private void NewSubtaskInputChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
                newSubtaskPlaceholder.Visibility = Visibility.Visible;
            else
                newSubtaskPlaceholder.Visibility = Visibility.Hidden;
        }
        private void NewSubtaskInputGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            newSubtaskInputBackground.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B448B"));
            newSubtaskPlaceholder.Visibility = Visibility.Hidden;
        }

        private void NewSubtaskInputLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            newCategoryInputBackground.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#343c72"));
            if (string.IsNullOrEmpty(textBox.Text))
                newSubtaskPlaceholder.Visibility = Visibility.Visible;
        }
    }
}
