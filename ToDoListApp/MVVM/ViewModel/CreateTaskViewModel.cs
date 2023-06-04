using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ToDoListApp.MVVM.ViewModel
{
    public class CreateTaskViewModel : ViewModelBase, IDataErrorInfo
    {
        private string _taskName;
        private string _taskDescription;
        private DateTime? _taskDeadline;
        private DateTime? _taskStartDate;
        private DateTime? _taskEndDate;
        private string _taskPriority;
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

        public string TaskDescription
        {
            get { return _taskDescription; }
            set
            {
                _taskDescription = value;
                OnPropertyChanged(nameof(TaskDescription));
                OnPropertyChanged(nameof(ValidationString));
            }
        }

        public DateTime? TaskDeadline
        {
            get { return _taskDeadline; }
            set
            {
                _taskDeadline = value;
                OnPropertyChanged(nameof(TaskDeadline));
                OnPropertyChanged(nameof(ValidationString));
            }
        }
        public DateTime? TaskStartDate
        {
            get { return _taskStartDate; }
            set
            {
                _taskStartDate = value;

                OnPropertyChanged(nameof(TaskStartDate));
                OnPropertyChanged(nameof(ValidationString));
            }
        }
        public DateTime? TaskEndDate
        {
            get { return _taskEndDate; }
            set
            {
                _taskEndDate = value;
                OnPropertyChanged(nameof(TaskEndDate));
                OnPropertyChanged(nameof(ValidationString));
            }
        }

        public string TaskPriority
        {
            get { return _taskPriority; }
            set
            {
                _taskPriority = value;
                OnPropertyChanged(nameof(TaskPriority));
                OnPropertyChanged(nameof(ValidationString));
            }
        }
        public ObservableCollection<Category> TaskCategories { get; set; }
        
        //wybór 
        private ComboBoxItem _selectedTaskPriorityItem;
        private bool _isAddCategoryChecked;
        private string _newCategoryName;
        public ComboBoxItem SelectedTaskPriorityItem
        {
            get { return _selectedTaskPriorityItem; }
            set
            {
                _selectedTaskPriorityItem = value;
                TaskPriority = _selectedTaskPriorityItem?.Content?.ToString();
                OnPropertyChanged(nameof(SelectedTaskPriorityItem));
                OnPropertyChanged(nameof(ValidationString));
            }
        }
        public ObservableCollection<Category> SelectedCategories { get; set; }
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
                    if (!string.IsNullOrEmpty(NewCategoryName))
                    {
                        TaskCategories.Add(new Category { Name = NewCategoryName, IsCustom = true });
                        NewCategoryName = string.Empty; // Wyczyść pole tekstowe
                    }
                }
            }
        }
        //Add new Category
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
        private ICommand _selectionChangedCommand;
        public ICommand SelectionChangedCommand
        {
            get { return _selectionChangedCommand; }
            set
            {
                _selectionChangedCommand = value;
                OnPropertyChanged(nameof(SelectionChangedCommand));
                OnPropertyChanged(nameof(ValidationString));
            }
        }
        //SubTaski
        public ObservableCollection<Subtask> Subtasks { get; set; }
        //repository
        private readonly ToDoDbContext _context;
        private readonly IUserRepository _userRepository;
        //Commands
        public ICommand AddSubtaskCommand { get; set; }
        public ICommand AddTaskCommand { get; set; }
        public ICommand AddCategoryCommand { get; set; }
        //użytkownik
        public string CurrentUsername { get; set; }
        private UserModel _loggedInUser;

        //Walidacja
        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(TaskName))
                {
                    if (string.IsNullOrEmpty(TaskName))
                    {
                        return "Task Name is required.";
                    }
                }
                else if (columnName == nameof(TaskPriority))
                {
                    if (string.IsNullOrEmpty(TaskPriority))
                    {
                        return "Priority is required.";
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
                var propertyErrors = typeof(CreateTaskViewModel)
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

            // Sprawdź, czy data zakończenia jest późniejsza niż data rozpoczęcia
            if (TaskEndDate.HasValue && TaskStartDate.HasValue && TaskEndDate < TaskStartDate)
            {
                errors.Add("End date cannot be earlier than start date.");
            }
            if (SelectedCategories.Count == 0)
            {
                errors.Add("At least one category is required.");
            }
            //nowa kategoria
            ObservableCollection<Category> userCategories = _userRepository.GetUserCategories(_userRepository.GetCurrentUsername());
            bool categoryExists = userCategories.Any(category => category.Name.Equals(NewCategoryName));

            if (categoryExists)
            {
                errors.Add("Category with the same name already exists.");
            }
            return errors;
        }

        public CreateTaskViewModel()
        {
            _context = new ToDoDbContext();
            _userRepository = new UserRepository(_context);
            // Pobierz bieżące nazwisko użytkownika
            CurrentUsername = _userRepository.GetCurrentUsername();
            _loggedInUser = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            SelectionChangedCommand = new ViewModelCommand(ListBoxSelectionChanged);

            TaskCategories = new ObservableCollection<Category>(_userRepository.GetUserCategories(CurrentUsername)
                .DistinctBy(category => category.Name)
                .ToList());

            SelectedCategories = new ObservableCollection<Category>();  // Inicjalizacja
            Subtasks = new ObservableCollection<Subtask>();
            // Przykładowe Kategorie
            if (!TaskCategories.Any(category => category.Name == "School"))
            {
                TaskCategories.Add(new Category { Name = "School", IsCustom = false, Owner= _loggedInUser.Id });
            }

            if (!TaskCategories.Any(category => category.Name == "Home"))
            {
                TaskCategories.Add(new Category { Name = "Home", IsCustom = false, Owner = _loggedInUser.Id });
            }

            if (!TaskCategories.Any(category => category.Name == "Work"))
            {
                TaskCategories.Add(new Category { Name = "Work", IsCustom = false, Owner = _loggedInUser.Id });
            }
            AddSubtaskCommand = new ViewModelCommand(ExecuteAddSubTaskCommand);
            AddTaskCommand = new ViewModelCommand(ExecuteAddTaskCommand);
            AddCategoryCommand = new ViewModelCommand(ExecuteAddCategoryCommand);
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
            Subtasks.Add(new Subtask());
        }
        private void ListBoxSelectionChanged(object obj)
        {
            var listBox = (ListBox)obj;
            var selectedItems = listBox.SelectedItems.Cast<Category>().ToList();

            if (SelectedCategories == null)
            {
                SelectedCategories = new ObservableCollection<Category>(selectedItems);
            }
            else
            {
                SelectedCategories.Clear();
                foreach (var item in selectedItems)
                {
                    SelectedCategories.Add(item);
                }
            }

            OnPropertyChanged(nameof(SelectedCategories));
            OnPropertyChanged(nameof(ValidationString));
        }


        private void ExecuteAddTaskCommand(object obj)
        {
            if (!string.IsNullOrEmpty(ValidationString))
            {
                // Jeśli istnieją błędy walidacyjne, przerwij wykonanie funkcji
                return;
            }
            var currentUser = _userRepository.GetByUsername(CurrentUsername);
            if (currentUser == null)
            {
                return;
            }
            var planner = _context.Planners.SingleOrDefault(p => p.User.Id == currentUser.Id);

            if (planner == null)
            {
                return;
            }
            var task = new MainTask
            {
                Name = TaskName,
                Description = TaskDescription,
                Deadline = TaskDeadline,
                StartDate = TaskStartDate,
                EndDate = TaskEndDate,
                Status = "To Do",
                Priority = TaskPriority,
                Categories = SelectedCategories.ToList(),
                Subtasks = Subtasks.ToList(),
                Planner= planner
            };
            _context.MainTasks.Add(task);
            _context.SaveChanges();
            Messenger.Publish("ShowAllTasksView");
        }
        

    }
}
