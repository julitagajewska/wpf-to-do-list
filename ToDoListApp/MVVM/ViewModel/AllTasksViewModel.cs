using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;
using ToDoListApp.MVVM.View;
using Microsoft.EntityFrameworkCore;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ToDoListApp.MVVM.ViewModel
{
    public class AllTasksViewModel :ViewModelBase
    {
        private readonly ToDoDbContext _context;
        private readonly MainTaskService _mainTaskService;
        private string _caption;
        private ObservableCollection<MainTask> _tasks;
        private UserModel _loggedInUser;
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
        public ObservableCollection<MainTask> Tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(Tasks));
            }
        }
        public ICommand ShowCreateTasksViewCommand { get; set; }
        public ICommand ShowDetailsTaskViewCommand { get; set; }

        public AllTasksViewModel(UserModel loggedInUser)
        {
            _context = new ToDoDbContext(); // Create an instance of ToDoDbContext
            _mainTaskService = new MainTaskService(_context);
            _loggedInUser = loggedInUser;
            ShowCreateTasksViewCommand = new ViewModelCommand(ExecuteShowCreateTasksViewCommand);
            ShowDetailsTaskViewCommand = new ViewModelCommand(ExecuteShowDetailsTaskViewCommand);
            // Load tasks from the database
            LoadTasks();
        }
        
        private void ExecuteShowCreateTasksViewCommand(object obj)
        {
            Messenger.Publish("ShowCreateTasksView");
            Caption = "Create Task";
        }
        private void ExecuteShowDetailsTaskViewCommand(object obj)
        {
            if (obj is MainTask selectedTask)
            {
                Messenger.Publish("ShowDetailsTaskView", selectedTask);
                Caption = "Task Details";
            }
        }

        private void LoadTasks()
        {
            if (_loggedInUser != null && _loggedInUser.Planner != null)
            {
                // Pobierz zadania związane z plannerm zalogowanego użytkownika
                Tasks = new ObservableCollection<MainTask>(_context.MainTasks
                    .Include(t => t.Categories)
                    .Where(t => t.PlannerId == _loggedInUser.PlannerId)
                    .ToList());
            }
            // Retrieve all tasks from the database
            else
            {
                Tasks = new ObservableCollection<MainTask>();
            }
            
            //Tasks = new ObservableCollection<MainTask>(_context.MainTasks.Include(t => t.Categories).ToList());
        }
        
    }   
    
}
    