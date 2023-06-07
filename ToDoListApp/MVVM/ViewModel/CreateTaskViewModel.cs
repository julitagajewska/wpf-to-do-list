using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using ToDoListApp.Data;
using ToDoListApp.Migrations;
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
        private string _newName;
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

        //Add new Category
        public string NewName
        {
            get { return _newName; }
            set
            {
                _newName = value;
                OnPropertyChanged(nameof(NewName));
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

        public ICommand EditSubtaskCommand { get; set; }
        //repository
        private readonly ToDoDbContext _context;
        private readonly IUserRepository _userRepository;
        //Commands
        public ICommand AddSubtaskCommand { get; set; }
        public ICommand DeleteSubtaskCommand { get; set; }
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
            var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            ObservableCollection<Category> userCategories = _userRepository.GetUserCategories(user);
            bool categoryExists = userCategories.Any(category => category.Name.Equals(NewName));

            if (categoryExists)
            {
                errors.Add("Category with the same name already exists.");
            }
            bool subTaskExist = Subtasks.Any(subTask => subTask.Name.Equals(NewName) && subTask!=SelectedSubtask);
            if (subTaskExist)
            {
                errors.Add("Subtask with the same name already exists.");
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
            var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            TaskCategories = new ObservableCollection<Category>(_userRepository.GetUserCategories(user)
                .DistinctBy(category => category.Name)
                .ToList());

            SelectedCategories = new ObservableCollection<Category>();  // Inicjalizacja
            Subtasks = new ObservableCollection<Subtask>();
            // Przykładowe Kategorie //Do usunięcia, jak rejestracja zostanie skończona
            /*if (!TaskCategories.Any(category => category.Name == "School"))
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
            }*/
            
            AddTaskCommand = new ViewModelCommand(ExecuteAddTaskCommand);
            AddCategoryCommand = new ViewModelCommand(ExecuteAddCategoryCommand);
            AddSubtaskCommand = new ViewModelCommand(ExecuteAddSubTaskCommand);
            EditSubtaskCommand = new ViewModelCommand(ExecuteEditSubtaskCommand);
            DeleteSubtaskCommand = new ViewModelCommand(ExecuteDeleteSubtaskCommand);

        }
        private void ExecuteAddCategoryCommand(object obj)
        {
            bool categoryExists = TaskCategories.Any(category =>
            category.Name.Equals(NewName, StringComparison.OrdinalIgnoreCase) &&
            category.IsCustom);

            if (!string.IsNullOrEmpty(NewName) && !categoryExists)
            {
                var category = new Category
                {
                    Name = NewName,
                    IsCustom = true,
                    Owner = _loggedInUser.Id // Przypisanie id Usera.
                };

                TaskCategories.Add(category);
                _context.Categories.Add(category);
                _context.SaveChanges();
                NewName = string.Empty;
            }
        }
        private void ExecuteAddSubTaskCommand(object obj)
        {
            bool subTaskExist = Subtasks.Any(subTask => subTask.Name.Equals(NewName) && subTask != SelectedSubtask);
            if (subTaskExist)
            {
                return;
            }
            if (!string.IsNullOrEmpty(NewName))
            {
                Subtasks.Add(new Subtask { Name= NewName, Status="To Do"});
                NewName = string.Empty;
            }
        }
        private void ExecuteEditSubtaskCommand(object obj)
        {
            bool subTaskExist = Subtasks.Any(subTask => subTask.Name.Equals(NewName) && subTask != SelectedSubtask);
            if (subTaskExist)
            {
                return;
            }
            if (!string.IsNullOrEmpty(NewName))
            {
                Subtask selectedSubtask = (Subtask)obj; // Pobierz wybrany Subtask z parametru
                                                        // Znajdź wybrany Subtask w kolekcji Subtasks
                Subtask subtaskToUpdate = Subtasks.FirstOrDefault(s => s == selectedSubtask);
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
                Subtasks.Remove(selectedSubtask);
            }
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
                Planner= planner,
            };
            foreach (var subtask in Subtasks)
            {
                task.Subtasks.Add(subtask);
                _context.Subtasks.Add(subtask);
            }

            _context.MainTasks.Add(task);
            _context.SaveChanges();
            Messenger.Publish("ShowAllTasksView");
        }
        

    }
}
