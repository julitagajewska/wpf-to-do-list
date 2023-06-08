using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;
using ToDoListApp.Store;

namespace ToDoListApp.MVVM.ViewModel
{
    internal class LoginViewModel : ViewModelBase
    {
        // Fields
        private string _username;
        private SecureString _password;
        private string _errorMessage;
        private bool _isViewVisible = true;
        private WelcomeViewModel welcomeViewModel;
        private readonly ToDoDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly WelcomePageVisibilityStore _visibilityStore;

        //private WelcomeViewModel _parent;

        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public SecureString Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }

            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public bool IsViewVisible
        {
            get
            {
                return _isViewVisible;
            }

            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }

        // Commands
        public ICommand LoginCommand { get; }
        public ICommand RecoverPasswordCommand { get; }
        public ICommand ShowPasswordCommand { get; }
        public ICommand RememberPasswordCommand { get; }


        // Constructor
        public LoginViewModel(WelcomePageVisibilityStore visibilityStore)
        {
            _context = new ToDoDbContext();
            _userRepository = new UserRepository(_context);

            LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            RecoverPasswordCommand = new ViewModelCommand(p => ExecuteRecoverPasswordCommand("", ""));

            //_parent = parent;
            _visibilityStore = visibilityStore;
        }

        private bool CanExecuteLoginCommand(object obj) // Validation here
        {
            bool validData;

            if (string.IsNullOrWhiteSpace(Username) || Username.Length < 3 || Password == null || Password.Length < 3)
            {
                validData = false;
            }
            else
            {
                validData = true;
            }

            return validData;
        }

        private void ExecuteLoginCommand(object obj)
        {
            var isValidUser = _userRepository.AuthenticateUser(new NetworkCredential(Username, Password));

            if (isValidUser)
            {
                Thread.CurrentPrincipal = new GenericPrincipal(
                    new GenericIdentity(Username), null);
                var planner = _userRepository.GetPlannerByUsername(Username); // Przykładowa metoda w UserRepository
                planner.CurrentDate = DateTime.Now; //Ustaw datę systemową
                //_parent.IsViewVisible = false;
                _visibilityStore.ChangeVisibilty(false);
            }
            else
            {
                ErrorMessage = "Invalid username or password";
                //_parent.IsViewVisible = true;
                _visibilityStore.ChangeVisibilty(true);
            }
        }

        private void ExecuteRecoverPasswordCommand(string username, string email)
        {
            throw new NotImplementedException();
        }
    }
}
