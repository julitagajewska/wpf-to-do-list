using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;
using Microsoft.EntityFrameworkCore;
using ToDoListApp.Store;

namespace ToDoListApp.MVVM.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly ToDoDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly WelcomePageVisibilityStore _visibilityStore;
        private string _errorMessage;
        private string _username;
        private string _password;
        private string _confirmPassword;
        private string _email;
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
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public ICommand LoginCommand { get; set; }
        public RegisterViewModel() 
        {
            _context = new ToDoDbContext();
            _userRepository = new UserRepository(_context);

            LoginCommand = new ViewModelCommand(ExecuteLoginCommand);
        }
        private void ExecuteLoginCommand(object obj)
        {

           
            //if(Password==ConfirmPassword)//sprawdź zgodność haseł
            var isValidUser = true; //tymczasowo
                //można dać tu hashowanie hasła
                if (isValidUser)
                {
                    var planner = new Planner();
                    //źle pobiera hasło i email
                    UserModel user = new UserModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Username = Username,
                        Password = Password,
                        Email = Email,
                        Planner = planner
                    };
                    _userRepository.Add(user);
                    Thread.CurrentPrincipal = new GenericPrincipal(
                        new GenericIdentity(Username), null);

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
    }
}
