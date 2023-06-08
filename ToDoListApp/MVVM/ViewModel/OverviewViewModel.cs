using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;
using ToDoListApp.MVVM.View;
using Microsoft.EntityFrameworkCore;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


namespace ToDoListApp.MVVM.ViewModel
{
    public class OverviewViewModel : ViewModelBase
    {
        private readonly ToDoDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly MainTaskService _mainTaskService;

        private ObservableCollection<MainTask> _todayTasks;
        private ObservableCollection<MainTask> _upcomingTasks;

        public ObservableCollection<MainTask> TodayTasks
        {
            get { return _todayTasks; }
            set
            {
                _todayTasks = value;
                OnPropertyChanged(nameof(TodayTasks));
            }
        }
        public ObservableCollection<MainTask> UpComingTasks
        {
            get { return _upcomingTasks; }
            set
            {
                _upcomingTasks = value;
                OnPropertyChanged(nameof(UpComingTasks));
            }
        }
        private int _todayTaskCount;
        public int TodayTaskCount
        {
            get { return _todayTaskCount; }
            set
            {
                _todayTaskCount = value;
                OnPropertyChanged(nameof(TodayTaskCount));
            }
        }
        private int _upcomingTaskCount;
        public int UpcomingTaskCount
        {
            get { return _upcomingTaskCount; }
            set
            {
                _upcomingTaskCount = value;
                OnPropertyChanged(nameof(UpcomingTaskCount));
            }
        }

        //Filtrowanie kategorii
        private UserModel _loggedInUser;
        private ObservableCollection<Category> _userCategories;
        private Category _selectedCategory;

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
                //FilterTasksByCategory();
            }
        }

        //Command
        public ICommand ShowDetailsTaskViewCommand { get; set; }
        public ICommand AllCategoriesButtonCommand { get; set; }
        public OverviewViewModel(UserModel loggedInUser)
        {
            _context = new ToDoDbContext();
            _mainTaskService = new MainTaskService(_context);
            _userRepository = new UserRepository(_context);
            _loggedInUser = loggedInUser;
            _loggedInUser.Planner.CurrentDate = DateTime.Now.Date;//00:00:00
            var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);

            ShowDetailsTaskViewCommand = new ViewModelCommand(ExecuteShowDetailsTaskViewCommand);
            AllCategoriesButtonCommand = new ViewModelCommand(ExecuteAllCategoriesButtonCommand);
            LoadTasks();
            LoadUserCategories(user);
        }
        private void ExecuteShowDetailsTaskViewCommand(object obj)
        {
            if (obj is MainTask selectedTask)
            {
                Messenger.Publish("ShowDetailsTaskView", selectedTask);
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
                _loggedInUser.Planner.CurrentDate = DateTime.Now.Date;//00:00:00
                TodayTasks = new ObservableCollection<MainTask>(_context.MainTasks
                    .Include(t => t.Categories)
                    .Where(t => t.PlannerId == _loggedInUser.PlannerId && t.PlannerDate ==_loggedInUser.Planner.CurrentDate)
                    .ToList());
                //UpComingTasks
                UpComingTasks = new ObservableCollection<MainTask>(_context.MainTasks
                    .Include(t => t.Categories)
                    .Where(t => t.PlannerId == _loggedInUser.PlannerId && t.PlannerDate!=null && t.PlannerDate != _loggedInUser.Planner.CurrentDate && (t.Status == "To Do" || t.Status == "In Progress"))
                    .ToList());
            }
            // Retrieve all tasks from the database
            else
            {
                TodayTasks = new ObservableCollection<MainTask>();
            }
            UpdateTaskCount();
        }
        private void LoadUserCategories(UserModel user)
        {
            UserCategories = new ObservableCollection<Category>(_userRepository.GetUserCategories(user)
                .DistinctBy(category => category.Name)
                .ToList());
        }
        private void UpdateTaskCount()
        {
            TodayTaskCount = TodayTasks.Count;
            UpcomingTaskCount = UpComingTasks.Count;
        }
    }
}
