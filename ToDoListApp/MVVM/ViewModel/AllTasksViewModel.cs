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
    public class AllTasksViewModel : ViewModelBase
    {
        private readonly ToDoDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly MainTaskService _mainTaskService;
        private string _caption;
        private ObservableCollection<MainTask> _tasks;
        private UserModel _loggedInUser;

        private ObservableCollection<Category> _userCategories;
        private Category _selectedCategory;
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
        public ObservableCollection<Category> UserCategories
        {
            get { return _userCategories; }
            set
            {
                _userCategories = value;
                OnPropertyChanged(nameof(UserCategories));
            }
        }

        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                FilterTasksByCategory();
            }
        }
        public ICommand ShowCreateTasksViewCommand { get; set; }
        public ICommand ShowDetailsTaskViewCommand { get; set; }
        public ICommand AllCategoriesButtonCommand { get; set; }
        public AllTasksViewModel(UserModel loggedInUser)
        {
            _context = new ToDoDbContext(); // Create an instance of ToDoDbContext
            _mainTaskService = new MainTaskService(_context);
            _userRepository = new UserRepository(_context);
            _loggedInUser = loggedInUser;
            ShowCreateTasksViewCommand = new ViewModelCommand(ExecuteShowCreateTasksViewCommand);
            ShowDetailsTaskViewCommand = new ViewModelCommand(ExecuteShowDetailsTaskViewCommand);
            AllCategoriesButtonCommand = new ViewModelCommand(ExecuteAllCategoriesButtonCommand);
            var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            // Load tasks from the database
            LoadTasks();
            LoadUserCategories(user);
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
        private void ExecuteAllCategoriesButtonCommand(object obj)
        {
            SelectedCategory = null;
            LoadTasks();
        }
        private void LoadTasks()
        {
            if (_loggedInUser != null && _loggedInUser.Planner != null)
            {
                // Pobierz zadania związane z plannerm zalogowanego użytkownika
                Tasks = new ObservableCollection<MainTask>(_context.MainTasks
                    .Include(t => t.Categories)
                    .Where(t => t.PlannerId == _loggedInUser.PlannerId && (t.Status == "To Do" || t.Status == "In Progress"))
                    .ToList());
            }
            // Retrieve all tasks from the database
            else
            {
                Tasks = new ObservableCollection<MainTask>();
            }

            //Tasks = new ObservableCollection<MainTask>(_context.MainTasks.Include(t => t.Categories).ToList());
        }
        private void LoadUserCategories(UserModel user)
        {
            UserCategories = new ObservableCollection<Category>(_userRepository.GetUserCategories(user)
                .DistinctBy(category => category.Name)
                .ToList());
        }
        private void FilterTasksByCategory()
        {
            LoadTasks();
            if (SelectedCategory != null)
            {
                // Filtruj zadania po wybranej kategorii
                Tasks = new ObservableCollection<MainTask>(_tasks
                    .Where(task => task.Categories.Contains(SelectedCategory))
                    .ToList());
            }
            else
            {
                // Jeśli żadna kategoria nie jest wybrana, wyświetl wszystkie zadania
                LoadTasks();
            }
        }

    }
}