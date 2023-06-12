using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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
    /// Interaction logic for DetailsTaskView.xaml
    /// </summary>
    public partial class DetailsTaskView : UserControl
    {
        public DetailsTaskView()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var viewModel = DataContext as DetailsTaskViewModel;
            var selectedSubtask = checkbox.DataContext as CheckBoxModel;

            viewModel.ToggleStatusCommand.Execute(selectedSubtask.Subtask);
        }
    }
}
