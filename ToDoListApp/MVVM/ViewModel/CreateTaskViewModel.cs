using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;

namespace ToDoListApp.MVVM.ViewModel
{
    public class CreateTaskViewModel : ViewModelBase
    {
        private string _taskName;
        private string _taskDescription;
        private DateTime? _taskDeadline;
        private DateTime? _taskStartDate;
        private DateTime? _taskEndDate;
        private string _taskStatus;
        private string _taskPriority;
        public string TaskName
        {
            get { return _taskName; }
            set
            {
                _taskName = value;
                OnPropertyChanged(nameof(TaskName));
            }
        }

        public string TaskDescription
        {
            get { return _taskDescription; }
            set
            {
                _taskDescription = value;
                OnPropertyChanged(nameof(TaskDescription));
            }
        }

        public DateTime? TaskDeadline
        {
            get { return _taskDeadline; }
            set
            {
                _taskDeadline = value;
                OnPropertyChanged(nameof(TaskDeadline));
            }
        }
        public DateTime? TaskStartDate
        {
            get { return _taskStartDate; }
            set
            {
                _taskStartDate = value;

                OnPropertyChanged(nameof(TaskStartDate));
            }
        }
        public DateTime? TaskEndDate
        {
            get { return _taskEndDate; }
            set
            {
                _taskEndDate = value;
                OnPropertyChanged(nameof(TaskEndDate));
            }
        }

        public string TaskStatus
        {
            get { return _taskStatus; }
            set
            {
                _taskStatus = value;
                OnPropertyChanged(nameof(TaskStatus));
            }
        }
        public string TaskPriority
        {
            get { return _taskPriority; }
            set
            {
                _taskPriority = value;
                OnPropertyChanged(nameof(TaskPriority));
            }
        }
        public ObservableCollection<Category> TaskCategories { get; set; }
        
        //wybór 
        private ComboBoxItem _selectedTaskStatusItem;
        private ComboBoxItem _selectedTaskPriorityItem;
        private bool _isAddCategoryChecked;
        private string _newCategoryName;

        public ComboBoxItem SelectedTaskStatusItem
        {
            get { return _selectedTaskStatusItem; }
            set
            {
                _selectedTaskStatusItem = value;
                TaskStatus = _selectedTaskStatusItem?.Content?.ToString();
                OnPropertyChanged(nameof(SelectedTaskStatusItem));
            }
        }

        public ComboBoxItem SelectedTaskPriorityItem
        {
            get { return _selectedTaskPriorityItem; }
            set
            {
                _selectedTaskPriorityItem = value;
                TaskPriority = _selectedTaskPriorityItem?.Content?.ToString();
                OnPropertyChanged(nameof(SelectedTaskPriorityItem));
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
            }
        }

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
        public CreateTaskViewModel()
        {
            _context = new ToDoDbContext();
            _userRepository = new UserRepository(_context);
            // Pobierz bieżące nazwisko użytkownika
            CurrentUsername = _userRepository.GetCurrentUsername();
            SelectionChangedCommand = new ViewModelCommand(ListBoxSelectionChanged);
            //TaskCategories = new ObservableCollection<Category>(_context.Categories.ToList());
            TaskCategories = new ObservableCollection<Category>(_userRepository.GetUserCategories(CurrentUsername)
                .DistinctBy(category => category.Name)
                .ToList());

            SelectedCategories = new ObservableCollection<Category>();  // Inicjalizacja
            Subtasks = new ObservableCollection<Subtask>();
            // Przykładowe Kategorie
            if (!TaskCategories.Any(category => category.Name == "School"))
            {
                TaskCategories.Add(new Category { Name = "School", IsCustom = false });
            }

            if (!TaskCategories.Any(category => category.Name == "Home"))
            {
                TaskCategories.Add(new Category { Name = "Home", IsCustom = false });
            }

            if (!TaskCategories.Any(category => category.Name == "Work"))
            {
                TaskCategories.Add(new Category { Name = "Work", IsCustom = false });
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
                TaskCategories.Add(new Category { Name = NewCategoryName, IsCustom = true });
                _userRepository.AddCategoryToUser(CurrentUsername, NewCategoryName);
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
        }
        private void ExecuteAddTaskCommand(object obj)
        {   
            var currentUser = _userRepository.GetByUsername(CurrentUsername);
            if (currentUser == null)
            {
                // Jeśli nie można znaleźć użytkownika, możesz podjąć odpowiednie działania.
                return;
            }
            var planner = _context.Planners.SingleOrDefault(p => p.User.Id == currentUser.Id);

            if (planner == null)
            {
                // Jeśli Planner nie istnieje, możesz podjąć odpowiednie działania, na przykład utworzyć nowy Planner dla bieżącego użytkownika.
                // Jeśli nie jest to wymagane, możesz rzucić wyjątek lub zwrócić informację użytkownikowi o braku Plannera.
                return;
            }
            var task = new MainTask
            {
                Name = TaskName,
                Description = TaskDescription,
                Deadline = TaskDeadline,
                StartDate = TaskStartDate,
                EndDate = TaskEndDate,
                Status = TaskStatus,
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
