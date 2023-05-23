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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ToDoListApp.MVVM.ViewModel
{
    public class AllTasksViewModel :ViewModelBase
    {
        private readonly ToDoDbContext _context;
        private readonly MainTaskService _mainTaskService;
        private string _caption;
        private ObservableCollection<MainTask> _tasks;
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
        public AllTasksViewModel()
        {
            _context = new ToDoDbContext(); // Create an instance of ToDoDbContext
            _mainTaskService = new MainTaskService(_context);
            ShowCreateTasksViewCommand = new ViewModelCommand(ExecuteShowCreateTasksViewCommand);
            // Load tasks from the database
            LoadTasks();
        }
        
        private void ExecuteShowCreateTasksViewCommand(object obj)
        {
            Messenger.Publish("ShowCreateTasksView");
            Caption = "Create Task";
        }

        private void LoadTasks()
        {
            // Retrieve all tasks from the database
            Tasks = new ObservableCollection<MainTask>(_context.MainTasks.ToList());
            foreach (var task in Tasks)
            {
                task.CategoryList = _mainTaskService.ConvertCategoriesToString(task.Categories);
            }
        }
        
    }   
    
}
    