﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.Model.Converters;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ToDoListApp.MVVM.ViewModel
{
    public class EditTaskViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly ToDoDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly MainTaskService _mainTaskService;
        public string CurrentUsername { get; set; }
        private MainTask _selectedTask;

        private string _newSubtaskName;
        private string _newCategoryName;
        public string NewCategoryName
        {
            get { return _newCategoryName; }
            set
            {
                _newCategoryName = value;
                OnPropertyChanged(nameof(NewCategoryName));
                OnPropertyChanged(nameof(ValidationString));
            }
        }
        public string NewSubtaskName
        {
            get { return _newSubtaskName; }
            set
            {
                _newSubtaskName = value;
                OnPropertyChanged(nameof(NewSubtaskName));
                OnPropertyChanged(nameof(ValidationString));
            }
        }
        public MainTask SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
                OnPropertyChanged(nameof(ValidationString));
            }
        }
         private string _taskName;
        public string TaskName
        {
            get { return _taskName; }
            set
            {
                _taskName = value;
                OnPropertyChanged(nameof(TaskName));
                OnPropertyChanged(nameof(ValidationString));
            }
        }
        //Status and Priority
        private List<string> Statuses;
        public List<string> statuses //list
        {
            get { return new List<string>() { "To Do", "In Progress", "Done" }; }
            set { Statuses = value; }
        }
        private List<string> Priorities;
        public List<string> priorities //list
        {
            get { return new List<string>() { "Low", "Medium", "High" }; }
            set { Priorities = value; }
        }
        private List<string> StatusesST;
        public List<string> statusesST //list
        {
            get { return new List<string>() { "To Do","Done" }; }
            set { StatusesST = value; }
        }
        public string status { get; set; } //from db
        public string priority { get; set; } //from db
        //Categories
        public ObservableCollection<Category> TaskCategories { get; set; } //List of User Available Categories
        public ObservableCollection<Category> SelectedCategories { get; set; }

        private ObservableCollection<Category> _listBoxSelectedItems;
        public ObservableCollection<Category> ListBoxSelectedItems
        {
            get { return _listBoxSelectedItems; }
            set
            {
                _listBoxSelectedItems = value;
                OnPropertyChanged(nameof(ListBoxSelectedItems));
                OnPropertyChanged(nameof(ValidationString));
            }
        }
        public List<int> SelectedCategoriesIds { get; set; }
        //Add new Category
        private bool _isAddCategoryChecked;
        private string _NewName;
        //User Categories
        public bool IsAddCategoryChecked
        {
            get { return _isAddCategoryChecked; }
            set
            {
                _isAddCategoryChecked = value;
                OnPropertyChanged(nameof(IsAddCategoryChecked));
                OnPropertyChanged(nameof(ValidationString));
                if (!value)
                {
                    // Jeśli przycisk został odznaczony, dodaj nową kategorię na liście
                    if (!string.IsNullOrEmpty(NewName))
                    {
                        TaskCategories.Add(new Category { Name = NewName, IsCustom = true });
                        NewName = string.Empty; // Wyczyść pole tekstowe
                    }
                }
            }
        }
        //Add new Category
        public string NewName
        {
            get { return _NewName; }
            set
            {
                _NewName = value;
                OnPropertyChanged(nameof(NewName));
                OnPropertyChanged(nameof(ValidationString));
            }
        }
        public ICommand AddCategoryCommand { get; set; }
        //Subtaski
        public ObservableCollection<Subtask> Subtasks { get; set; }
        private Subtask _selectedSubtask;
        public Subtask SelectedSubtask
        {
            get { return _selectedSubtask; }
            set
            {
                _selectedSubtask = value;
                OnPropertyChanged(nameof(SelectedSubtask));
                NewName = SelectedSubtask?.Name;
            }
        }
        public ObservableCollection<Subtask> TaskSubTasks { get; set; } //List of User Available Categories
        //Walidacja

        public string this[string columnName]
        {
            get
            {
                if (SelectedTask != null)
                {
                    if (columnName == "TaskName")
                    {
                        if (string.IsNullOrEmpty(SelectedTask.Name))
                        {
                            return "Task Name is required.";
                        }
                    }
                    else if (columnName == "TaskPriority")
                    {
                        if (string.IsNullOrEmpty(priority))
                        {
                            return "Priority is required.";
                        }
                    }
                    else if (columnName == "TaskStatus")
                    {
                        if (string.IsNullOrEmpty(status))
                        {
                            return "Status is required.";
                        }
                    }
                }
                
                return null;
            }
        }
        public string Error => throw new NotImplementedException();
        public string ValidationString
        {
            get
            {
                // Sprawdź, czy istnieją jakieś błędy walidacji dla poszczególnych właściwości
                var propertyErrors = typeof(EditTaskViewModel)
                    .GetProperties()
                    .Where(p => this[p.Name] != null)
                    .Select(p => this[p.Name]);

                // Sprawdź, czy istnieją błędy walidacji dla innych warunków
                var otherErrors = GetOtherValidationErrors(); // Zaimplementuj tę metodę, aby zwracała inne błędy walidacji

                // Połącz wszystkie błędy walidacji w jeden ciąg tekstowy
                var validationErrors = propertyErrors.Concat(otherErrors);
                return string.Join(Environment.NewLine, validationErrors);
            }
        }
        private IEnumerable<string> GetOtherValidationErrors()
        {
            List<string> errors = new List<string>();

            if(SelectedTask != null) {
                // Sprawdź, czy data zakończenia jest późniejsza niż data rozpoczęcia
                if (SelectedTask.EndDate.HasValue && SelectedTask.StartDate.HasValue && SelectedTask.EndDate < SelectedTask.StartDate)
                {
                    errors.Add("End date cannot be earlier than start date.");
                }
                if ((SelectedTask.Deadline.HasValue && SelectedTask.Deadline < _loggedInUser.Planner.CurrentDate) || SelectedTask.PlannerDate.HasValue && SelectedTask.PlannerDate < _loggedInUser.Planner.CurrentDate)
                {
                    errors.Add("You cannot add date before today.");
                }
                if (ListBoxSelectedItems != null && ListBoxSelectedItems.Count == 0)
                {
                    errors.Add("At least one category is required.");
                }
                //nowa kategoria
                var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
                ObservableCollection<Category> userCategories = _userRepository.GetUserCategories(user);
                bool categoryExists = userCategories.Any(category => category.Name.Equals(NewName));

                if (categoryExists)
                {
                    errors.Add("Category with the same name already exists.");
                }

                bool subTaskExist = TaskSubTasks.Any(subTask => subTask.Name.Equals(NewName) && subTask!=SelectedSubtask);
                if (subTaskExist)
                {
                    errors.Add("Subtask with the same name already exists.");
                }
            }
            return errors;
        }


        //użytkownik
        private UserModel _loggedInUser;
        public ICommand EditTaskCommand { get; set; }
        public ICommand AddSubtaskCommand { get; set; }
        public ICommand EditSubtaskCommand { get; set; }
        public ICommand DeleteSubtaskCommand { get; set; }
        public EditTaskViewModel(MainTask selectedTask)
        {
            _context = new ToDoDbContext();
            _mainTaskService = new MainTaskService(_context);
            _userRepository = new UserRepository(_context);
            CurrentUsername = _userRepository.GetCurrentUsername();
            _loggedInUser = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            _loggedInUser.Planner.CurrentDate = DateTime.Now.Date;
            SelectedTask = _context.MainTasks.Include(t => t.Categories).Include(t => t.Subtasks).FirstOrDefault(t => t.Id == selectedTask.Id);
            EditTaskCommand = new ViewModelCommand(EditTask);
            status = selectedTask.Status;
            priority = selectedTask.Priority;
            TaskSubTasks = new ObservableCollection<Subtask>(selectedTask.Subtasks);
            //Categories
            var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            TaskCategories = new ObservableCollection<Category>(_userRepository.GetUserCategories(user)
                .DistinctBy(category => category.Name)
                .ToList());

            SelectedCategories = new ObservableCollection<Category>();

            SelectedCategoriesIds = selectedTask.Categories.Select(c => c.Id).ToList();

            foreach (var category in TaskCategories)
            {
                var isSelected = SelectedCategoriesIds.Contains(category.Id);
                if (isSelected)
                {
                    SelectedCategories.Add(category);
                }
            }
            ListBoxSelectedItems = SelectedCategories;
            AddCategoryCommand = new ViewModelCommand(ExecuteAddCategoryCommand);
            AddSubtaskCommand = new ViewModelCommand(ExecuteAddSubTaskCommand);
            EditSubtaskCommand = new ViewModelCommand(ExecuteEditSubtaskCommand);
            DeleteSubtaskCommand = new ViewModelCommand(ExecuteDeleteSubtaskCommand);
        }
        private void ExecuteAddCategoryCommand(object obj)
        {
            bool categoryExists = TaskCategories.Any(category =>
            category.Name.Equals(NewCategoryName, StringComparison.OrdinalIgnoreCase) &&
            category.IsCustom);

            if (!string.IsNullOrEmpty(NewCategoryName) && !categoryExists)
            {
                var category = new Category
                {
                    Name = NewCategoryName,
                    IsCustom = true,
                    Owner = _loggedInUser.Id // Przypisanie id Usera.
                };
                var otherValidationErrors = GetOtherValidationErrors();
                if (otherValidationErrors.Any())
                {
                    return;
                }
                TaskCategories.Add(category);
                _context.Categories.Add(category);
                _context.SaveChanges();
                NewCategoryName = string.Empty;
            }
        }
        private void ExecuteAddSubTaskCommand(object obj)
        {
            bool subTaskExist = TaskSubTasks.Any(subTask => subTask.Name.Equals(NewSubtaskName) && subTask != SelectedSubtask);
            if (subTaskExist)
            {
                return;
            }
            if (!string.IsNullOrEmpty(NewSubtaskName))
            {
                TaskSubTasks.Add(new Subtask { Name = NewSubtaskName, Status = "To Do" });
                NewSubtaskName = string.Empty;
            }
        }
        private void ExecuteEditSubtaskCommand(object obj)
        {
            if (!string.IsNullOrEmpty(NewName))
            {
                bool subTaskExist = TaskSubTasks.Any(subTask => subTask.Name.Equals(NewName) && subTask != SelectedSubtask);
                if (subTaskExist)
                {
                    return;
                }
                Subtask selectedSubtask = (Subtask)obj; // Pobierz wybrany Subtask z parametru
                                                        // Znajdź wybrany Subtask w kolekcji Subtasks
                Subtask subtaskToUpdate = TaskSubTasks.FirstOrDefault(s => s == selectedSubtask);
                if (subtaskToUpdate != null)
                {
                    subtaskToUpdate.Name = NewName; // Zaktualizuj nazwę Subtaska
                    NewName = string.Empty; // Wyczyść pole tekstowe
                }
            }
            
        }
        private void ExecuteDeleteSubtaskCommand(object obj)
        {
            Subtask selectedSubtask = (Subtask)obj;

            if (selectedSubtask != null)
            {
                TaskSubTasks.Remove(selectedSubtask);
            }
        }
        private void EditTask(object obj)
        {
            if (SelectedTask != null)
            {
                if (!string.IsNullOrEmpty(ValidationString))
                {
                    // Jeśli istnieją błędy walidacyjne, przerwij wykonanie funkcji
                    return;
                }
                SelectedTask.Categories.Clear();

                foreach (var category in ListBoxSelectedItems)
                {
                    SelectedTask.Categories.Add(category);
                }
                SelectedTask.Subtasks.Clear();
                foreach (var subTask in TaskSubTasks)
                {
                    SelectedTask.Subtasks.Add(subTask);
                }
                _mainTaskService.UpdateMainTask(SelectedTask);
                _context.SaveChanges();
                SelectedTask = null;
                Messenger.Publish("ShowAllTasksView");
            }
        }
    }
}
