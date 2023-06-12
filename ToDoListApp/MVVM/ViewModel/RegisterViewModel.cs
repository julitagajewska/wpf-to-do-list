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
using System.Security;
using System.Runtime.InteropServices;

namespace ToDoListApp.MVVM.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly ToDoDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly WelcomePageVisibilityStore _visibilityStore;
        private string _errorMessage;

        private string _username;
        private string _usernameError;

        private SecureString _password;
        private string _passwordError;

        private SecureString _confirmPassword;
        private string _confirmPasswordError;

        private string _email;
        private string _emailError;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern int lstrcmp(IntPtr lpString1, IntPtr lpString2);
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
                ValidateUsername();
            }
        }
        public string UsernameError 
        {   
            get => _usernameError;
            set
            {
                _usernameError = value;
                OnPropertyChanged(nameof(UsernameError));
            }
        }

        private void ValidateUsername()
        {
            UsernameError = "";
            if (Username.ToString().Length < 5)
            {
                UsernameError = "Username is too short";
            }

            if (Username.ToString().Length == 0)
            {
                UsernameError = "Username is required";
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
                ValidatePassword();
            }
        }

        private void ValidatePassword()
        {
            PasswordError = "";
            if(Password.Length < 8)
            {
                PasswordError = "Password is too short";
            }
            if (Password.Length == 0)
            {
                PasswordError = "Password is required";
            }
        }

        public SecureString ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
                ValidateConfirmPassword();
            }
        }

        private void ValidateConfirmPassword()
        {
            ConfirmPasswordError = "";
            var passwordBstr = Marshal.SecureStringToBSTR(Password);
            var confirmPasswordBstr = Marshal.SecureStringToBSTR(ConfirmPassword);

            if(lstrcmp(passwordBstr, confirmPasswordBstr) != 0)
            {
                ConfirmPasswordError = "Passwords don't match";
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
                ValidateEmail();
            }
        }

        private void ValidateEmail()
        {
            EmailError = "";
            if (Email.ToString().Length < 5)
            {
                // Change to regex
                EmailError = "Username is too short";
            }

            if (Email.ToString().Length == 0)
            {
                EmailError = "Email is required";
            }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand ValidatePasswordCommand { get; set; }
        public string PasswordError 
        { 
            get => _passwordError;
            set
            {
                _passwordError = value;
                OnPropertyChanged(nameof(PasswordError));
            }
        }
        public string ConfirmPasswordError
        {
            get => _confirmPasswordError;
            set
            {
                _confirmPasswordError = value;
                OnPropertyChanged(nameof(ConfirmPasswordError));
            }
        }
        public string EmailError 
        { 
            get => _emailError;
            set
            {
                _emailError = value;
                OnPropertyChanged(nameof(EmailError));
            }
        }

        public RegisterViewModel() 
        {
            _context = new ToDoDbContext();
            _userRepository = new UserRepository(_context);

            LoginCommand = new ViewModelCommand(ExecuteLoginCommand);
            ValidatePasswordCommand = new ViewModelCommand(ExecuteValidatePasswordCommand);
        }

        private void ExecuteValidatePasswordCommand(object obj)
        {
            // Console.WriteLine(obj);
            // ValidatePassword((String)obj);
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
                        Password = Password.ToString(),
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
