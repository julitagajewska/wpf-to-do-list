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
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            var selectedItems = listBox.SelectedItems.Cast<Category>().ToList();
            var viewModel = (CreateTaskViewModel)DataContext;
            viewModel.SelectedCategories = new ObservableCollection<Category>(selectedItems);
        }
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            subtasksListBox.Items.Refresh();
        }
    }
}
