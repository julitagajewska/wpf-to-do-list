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
    /// Interaction logic for EditTaskView.xaml
    /// </summary>
    public partial class EditTaskView : UserControl
    {
        public EditTaskView()
        {
            InitializeComponent();
            ListBoxCategories.SelectionChanged += ListBox_SelectionChanged;
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as EditTaskViewModel;
            if (viewModel != null)
            {
                if (viewModel.ListBoxSelectedItems != null)
                {
                    viewModel.ListBoxSelectedItems = new ObservableCollection<Category>(ListBoxCategories.SelectedItems.OfType<Category>());

                }
                else
                {
                    viewModel.ListBoxSelectedItems = new ObservableCollection<Category>();
                }

                foreach (var selectedItem in ListBoxCategories.SelectedItems)
                {
                    viewModel.ListBoxSelectedItems.Add((Category)selectedItem);
                }
            }
        }

        private void ListBoxCategories_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as EditTaskViewModel;
            if (viewModel != null)
            {
                var selectedItems = viewModel.ListBoxSelectedItems.ToList();

                foreach (var category in selectedItems)
                {
                    ListBoxCategories.SelectedItems.Add(category);
                }
            }
        }

    }
}
