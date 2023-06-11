using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.View;
using ToDoListApp.MVVM.Model.Services;
using ToDoListApp.Data;
using Microsoft.VisualBasic.ApplicationServices;
using ToDoListApp.Store;

namespace ToDoListApp.MVVM.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        // Fields
        private UserAccountModel _currentUserAccount;
        private readonly ToDoDbContext _context;
        private IUserRepository _userRepository;
        private ViewModelBase _currentChildView;
        private string _caption;
        private MainViewStore store;

        private string _addTaskVisibility;
        private string _username;

        public UserAccountModel CurrentUserAccount
        {
            get
            {
                return _currentUserAccount;
            }

            set
            {
                _currentUserAccount = value;
                OnPropertyChanged(nameof(CurrentUserAccount));
            }
        }

        public ViewModelBase CurrentChildView {
            get
            {
                return _currentChildView;
            }
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            } 
        }
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

        public string addTaskVisibility
        {
            get
            {
                return _addTaskVisibility;
            }
            set
            {
                _addTaskVisibility = value;
                OnPropertyChanged(nameof(addTaskVisibility));
            }
        }

        public string username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(username));
            }
        }


        // Commands
        public ICommand ShowOverviewViewCommand { get; set; }
        public ICommand ShowAllTasksViewCommand { get; set; }
        public ICommand ShowArchiveViewCommand { get; set; }
        public ICommand ShowProfileViewCommand { get; set; }
        public ICommand ShowCategoryPanelViewCommand { get; set; }
        public ICommand LogOutCommand { get; set; }
        public ICommand ShowArchivedTaskDetailsViewCommand { get; set; }

        public MainViewModel()
        {
            _context = new ToDoDbContext(); // Create an instance of ToDoDbContext
            _userRepository = new UserRepository(_context); // Pass the ToDoDbContext instance to the UserRepository constructor
            CurrentUserAccount = new UserAccountModel();

            Messenger.Subscribe("ShowCreateTasksView", ShowCreateTasksView);
            Messenger.Subscribe("ShowAllTasksView", ExecuteShowAllTasksViewCommand);
            Messenger.Subscribe("ShowDetailsTaskView", ShowDetailsTaskView);
            Messenger.Subscribe("ShowCategoryPanelView", ExecuteShowCategoryPanelView);
            Messenger.Subscribe("ShowEditTasksView", ExecuteShowEditTasksView);
            Messenger.Subscribe("ShowEditCategoryView", ExecuteShowEditCategoryView);
            Messenger.Subscribe("ShowArchivedTaskDetailsView", ExecuteShowArchivedTaskDetailsView);

            // Initialize commands
            ShowOverviewViewCommand = new ViewModelCommand(ExecuteShowOverviewViewCommand);
            ShowAllTasksViewCommand = new ViewModelCommand(ExecuteShowAllTasksViewCommand);
            ShowArchiveViewCommand = new ViewModelCommand(ExecuteShowArchiveViewCommand);
            ShowProfileViewCommand = new ViewModelCommand(ExecuteShowProfileViewCommand);
            ShowCategoryPanelViewCommand = new ViewModelCommand(ExecuteShowCategoryPanelView);
            ShowArchivedTaskDetailsViewCommand = new ViewModelCommand(ExecuteShowArchivedTaskDetailsView);

            LogOutCommand = new ViewModelCommand(ExecuteLogOutcommand);

            store = new MainViewStore();

            store.AddTaskVisibilityChanged += OnAddTaskVisibilityChanged;

            // Deafult view
            ExecuteShowOverviewViewCommand(null);

            LoadCurrentUserData();
        }

        private void ExecuteShowArchivedTaskDetailsView(object payload)
        {
            if (payload is MainTask selectedTask)
            {
                Caption = "Archived task details";
                CurrentChildView = new ArchivedTaskDetailsViewModel(selectedTask);
            }
        }

        private void OnAddTaskVisibilityChanged(string value)
        {
            addTaskVisibility = value;
        }

        private void ExecuteLogOutcommand(object obj)
        {
            Thread.CurrentPrincipal = null;

            var loginView = new WelcomeView();
            loginView.Show();

            var mainView = new MainView();
            mainView.Close();

        }

        private void ExecuteShowOverviewViewCommand(object obj)
        {
            var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            CurrentChildView = new OverviewViewModel(user);

            Caption = "Overview";
            addTaskVisibility = "Visible";
            username = CurrentUserAccount.Username;
        }

        private void ExecuteShowAllTasksViewCommand(object obj)
        {
            var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            CurrentChildView = new AllTasksViewModel(user);
            Caption = "All tasks";
            addTaskVisibility = "Visible";
            username = CurrentUserAccount.Username;
        }

        private void ExecuteShowProfileViewCommand(object obj)
        {
            CurrentChildView = new ProfileViewModel();
            Caption = "User's profile";
            addTaskVisibility = "Hidden";
            username = CurrentUserAccount.Username;
        }

        private void ExecuteShowArchiveViewCommand(object obj)
        {
            var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            CurrentChildView = new ArchiveViewModel(user);
            Caption = "Archive";
            addTaskVisibility = "Hidden";
            username = CurrentUserAccount.Username;
        }
        private void ShowCreateTasksView(object parameter)
        {
            CurrentChildView = new CreateTaskViewModel();
            Caption = "Create Task";
            addTaskVisibility = "Hidden";
            username = CurrentUserAccount.Username;
        }
        private void ShowDetailsTaskView(object payload)
        {
            if (payload is MainTask selectedTask)
            {
                addTaskVisibility = "Hidden";
                // Caption = selectedTask.Name;
                Caption = "Task details";
                CurrentChildView = new DetailsTaskViewModel(selectedTask);
            }
        }
        private void ExecuteShowEditTasksView(object payload)
        {
            if (payload is MainTask selectedTask)
            {
                Caption = selectedTask.Name;
                CurrentChildView = new EditTaskViewModel(selectedTask);
            }

        }
        private void ExecuteShowEditCategoryView(object payload)
        {
            var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            if (payload is Category selectedCategory)
            {
                Caption = $"Edit category: {selectedCategory.Name}";
                CurrentChildView = new EditCategoryViewModel(selectedCategory);
            }

        }
        private void ExecuteShowCategoryPanelView(object obj)//TUTAJ OGARNIJ
        {
            var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            CurrentChildView = new CategoryPanelViewModel(user);
            Caption = "Categories";
            addTaskVisibility = "Hidden";
        }
        private void LoadCurrentUserData()
        {
            var identity = Thread.CurrentPrincipal?.Identity;
            if(identity != null){
                var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
                if (user != null)
                {
                    CurrentUserAccount.Username = user.Username;
                    CurrentUserAccount.DisplayName = $"Welcome {user.Username}!";
                    CurrentUserAccount.ProfilePicture = null;
                }
                else
                {
                    CurrentUserAccount.DisplayName = "Invalid user, not logged in";
                    //MessageBox.Show("Invalid user, not logged in");
                    //Application.Current.Shutdown();
                }
            }
            
        }
    }
}
