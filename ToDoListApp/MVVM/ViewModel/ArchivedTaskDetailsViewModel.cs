using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.Model.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Windows;
using System.Windows.Input;

namespace ToDoListApp.MVVM.ViewModel
{
    public class ArchivedTaskDetailsViewModel : ViewModelBase
    {
        private MainTask _selectedTask;
        private readonly ToDoDbContext _context;
        private readonly IMainTaskService _mainTaskService;
        private ObservableCollection<CheckBoxModel> _checkboxes;

        public MainTask SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }

        public ObservableCollection<CheckBoxModel> Checkboxes
        {
            get { return _checkboxes; }
            set
            {
                _checkboxes = value;
                OnPropertyChanged(nameof(Checkboxes));
            }
        }

        public ICommand EditTasksViewCommand { get; set; }
        public ICommand DeleteTasksViewCommand { get; set; }

        public ArchivedTaskDetailsViewModel(MainTask selectedTask)
        {
            _context = new ToDoDbContext();
            _mainTaskService = new MainTaskService(_context);
            SelectedTask = _context.MainTasks.Include(t => t.Categories).Include(t => t.Subtasks).FirstOrDefault(t => t.Id == selectedTask.Id);
            _checkboxes = new ObservableCollection<CheckBoxModel>();

            EditTasksViewCommand = new ViewModelCommand(ShowEditTaskViewCommand);
            DeleteTasksViewCommand = new ViewModelCommand(ExecuteDeleteTaskCommand);

            foreach (Subtask task in SelectedTask.Subtasks)
            {
                bool status = false;
                if(task.Status == "Done") status = true;
                _checkboxes.Add(new CheckBoxModel { ButtonText = task.Name, Checked = status });
            }
        }

        private void ShowEditTaskViewCommand(object obj)
        {
            if (SelectedTask != null)
            {
                Messenger.Publish("ShowEditTasksView", SelectedTask);
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
