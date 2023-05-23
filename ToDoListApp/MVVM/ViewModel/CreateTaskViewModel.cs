using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string _taskPriority;
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
        public ObservableCollection<Subtask> Subtasks { get; set; }
        //repository
        private readonly ToDoDbContext _context;
        private readonly IUserRepository _userRepository;
        //Commands
        public ICommand AddSubtaskCommand { get; set; }
        public ICommand AddTaskCommand { get; set; }
        //użytkownik
        public string CurrentUsername { get; set; }

        public CreateTaskViewModel()
        {
            _context = new ToDoDbContext();
            _userRepository = new UserRepository(_context);
            // Pobierz bieżące nazwisko użytkownika
            CurrentUsername = _userRepository.GetCurrentUsername();

            TaskCategories = new ObservableCollection<Category>(_context.Categories.ToList());
            Subtasks = new ObservableCollection<Subtask>();

            AddSubtaskCommand = new ViewModelCommand(ExecuteAddSubTaskCommand);
            AddTaskCommand = new ViewModelCommand(ExecuteAddTaskCommand);
        }
        private void ExecuteAddSubTaskCommand(object obj)
        {
            Subtasks.Add(new Subtask());
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
                Categories = TaskCategories.ToList(),
                Subtasks = Subtasks.ToList(),
                Planner= planner
            };

            _context.MainTasks.Add(task);
            _context.SaveChanges();
            Messenger.Publish("ShowAllTasksView");
        }
        

    }
}
