using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;
using System.Windows.Input;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;

namespace ToDoListApp.MVVM.ViewModel
{
    public class DetailsTaskViewModel : ViewModelBase
    {
        private MainTask _selectedTask;
        private readonly ToDoDbContext _context;
        private readonly IMainTaskService _mainTaskService;
        private string _caption;
        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }
        public MainTask SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }
        public ICommand EditTasksViewCommand { get; set; }
        public ICommand DeleteTasksViewCommand { get; set; }
        public DetailsTaskViewModel(MainTask selectedTask)
        {
            _context = new ToDoDbContext();
            _mainTaskService = new MainTaskService(_context);
            SelectedTask = _context.MainTasks.Include(t => t.Categories).FirstOrDefault(t => t.Id == selectedTask.Id);

            EditTasksViewCommand = new ViewModelCommand(ShowEditTaskViewCommand);
            DeleteTasksViewCommand = new ViewModelCommand(ExecuteDeleteTaskCommand);
        }
        private void ShowEditTaskViewCommand(object obj)
        {
            if (SelectedTask!=null)
            {
                Messenger.Publish("ShowEditTasksView", SelectedTask);
                Caption = "Edit Task";
            }
        }
        private void ExecuteDeleteTaskCommand(object obj)
        {
            if (obj is MainTask taskToDelete)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this Task?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _mainTaskService.DeleteTask(taskToDelete);
                    _context.SaveChanges();
                    Messenger.Publish("ShowAllTasksView");
                }
            }
        }
    }
}
