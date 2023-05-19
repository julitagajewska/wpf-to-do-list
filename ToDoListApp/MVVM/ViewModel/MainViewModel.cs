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

namespace ToDoListApp.MVVM.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        // Fields
        private UserAccountModel _currentUserAccount;
        private ViewModelBase _currentChildView;
        private string _caption;
        private IUserRepository _userRepository;
        private readonly ToDoDbContext _context;

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

        // Commands
        public ICommand ShowOverviewViewCommand { get; set; }
        public ICommand ShowAllTasksViewCommand { get; set; }
        public ICommand ShowArchiveViewCommand { get; set; }
        public ICommand ShowProfileViewCommand { get; set; }
        public ICommand LogOutCommand { get; set; }

        public MainViewModel()
        {
            _context = new ToDoDbContext(); // Create an instance of ToDoDbContext
            _userRepository = new UserRepository(_context); // Pass the ToDoDbContext instance to the UserRepository constructor
            CurrentUserAccount = new UserAccountModel();

            // Initialize commands
            ShowOverviewViewCommand = new ViewModelCommand(ExecuteShowOverviewViewCommand);
            ShowAllTasksViewCommand = new ViewModelCommand(ExecuteShowAllTasksViewCommand);
            ShowArchiveViewCommand = new ViewModelCommand(ExecuteShowArchiveViewCommand);
            ShowProfileViewCommand = new ViewModelCommand(ExecuteShowProfileViewCommand);

            LogOutCommand = new ViewModelCommand(ExecuteLogOutcommand);

            // Deafult view
            ExecuteShowOverviewViewCommand(null);

            LoadCurrentUserData();
        }

        private void ExecuteLogOutcommand(object obj)
        {
            Thread.CurrentPrincipal = null;

            var loginView = new LoginView();
            loginView.Show();

            var mainView = new MainView();
            mainView.Close();

        }

        private void ExecuteShowOverviewViewCommand(object obj)
        {
            CurrentChildView = new OverviewViewModel();
            Caption = "Overview";
        }

        private void ExecuteShowAllTasksViewCommand(object obj)
        {
            CurrentChildView = new AllTasksViewModel();
            Caption = "All tasks";
        }

        private void ExecuteShowProfileViewCommand(object obj)
        {
            CurrentChildView = new ProfileViewModel();
            Caption = CurrentUserAccount.Username;
        }

        private void ExecuteShowArchiveViewCommand(object obj)
        {
            CurrentChildView = new ArchiveViewModel();
            Caption = "Archive";
        }


        private void LoadCurrentUserData()
        {   
            var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            if(user != null)
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
