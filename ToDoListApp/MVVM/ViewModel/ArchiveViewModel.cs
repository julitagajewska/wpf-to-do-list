﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;
using ToDoListApp.MVVM.Model;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace ToDoListApp.MVVM.ViewModel
{
    public class ArchiveViewModel : ViewModelBase
    {
        private readonly ToDoDbContext _context; 
        private readonly IUserRepository _userRepository;
        private readonly MainTaskService _mainTaskService;
        private string _caption;
        private ObservableCollection<MainTask> _tasks;
        private UserModel _loggedInUser;
        private ObservableCollection<Category> _userCategories;
        private Category _selectedCategory;
        private int _taskCount;
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
        public int TaskCount
        {
            get { return _taskCount; }
            set
            {
                _taskCount = value;
                OnPropertyChanged(nameof(TaskCount));
            }
        }
        public ICommand ShowDetailsTaskViewCommand { get; set; }
        public ICommand AllCategoriesButtonCommand { get; set; }
        public ICommand FilterTasksCommand { get; set; }
        public ICommand LoadTasksCommand { get; set; }

        public ArchiveViewModel(UserModel loggedInUser)
        {
            _context = new ToDoDbContext(); // Create an instance of ToDoDbContext
            _mainTaskService = new MainTaskService(_context);
            _userRepository = new UserRepository(_context);
            _loggedInUser = loggedInUser;
            ShowDetailsTaskViewCommand = new ViewModelCommand(ExecuteShowDetailsTaskViewCommand);
            AllCategoriesButtonCommand = new ViewModelCommand(ExecuteAllCategoriesButtonCommand);
            FilterTasksCommand = new ViewModelCommand(ExecuteFilterTasksCommand);
            LoadTasksCommand = new ViewModelCommand(ExecuteLoadTasksCommand);

            // Load tasks from the database
            LoadTasks();
            var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            LoadUserCategories(user);
        }

        private void ExecuteLoadTasksCommand(object obj)
        {
            LoadTasks();
        }

        private void ExecuteFilterTasksCommand(object obj)
        {
            String searchInput = (String)obj;

            if (searchInput != "")
            {
                Tasks = new ObservableCollection<MainTask>(_context.MainTasks
                    .Where(x => x.Name.ToLower().Contains(searchInput.ToLower()) && x.Status == "Done" && x.PlannerId == _loggedInUser.PlannerId)
                    .ToList());
            }
            else
            {
                LoadTasks();
            }

        }

        private void UpdateTaskCount()
        {
            TaskCount = Tasks.Count;
        }
        private void ExecuteShowDetailsTaskViewCommand(object obj)
        {
            if (obj is MainTask selectedTask)
            {
                Messenger.Publish("ShowArchivedTaskDetailsView", selectedTask);
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
                _loggedInUser.Planner.CurrentDate = DateTime.Now.Date;
                Tasks = new ObservableCollection<MainTask>(_context.MainTasks
                    .Include(t => t.Categories)
                    //.Where(t => t.PlannerId == _loggedInUser.PlannerId && t.Status == "Done" && t.PlannerDate != _loggedInUser.Planner.CurrentDate)
                    .Where(t => t.PlannerId == _loggedInUser.PlannerId && t.Status == "Done")
                    .ToList());
            }
            // Retrieve all tasks from the database
            else
            {
                Tasks = new ObservableCollection<MainTask>();
            }

            UpdateTaskCount();
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
