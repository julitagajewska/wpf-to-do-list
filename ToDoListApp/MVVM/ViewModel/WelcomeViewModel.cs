using GalaSoft.MvvmLight.Command;
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
using System.Xml.Serialization;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;
using ToDoListApp.MVVM.View;
using ToDoListApp.Store;

namespace ToDoListApp.MVVM.ViewModel
{
    public class WelcomeViewModel : ViewModelBase
    {

        // Fields
        private string _username;
        private SecureString _password;
        private string _errorMessage;
        private bool _isViewVisible = true;
        private readonly ToDoDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly WelcomePageVisibilityStore visibilityStore;

        private ViewModelBase _currentChildView;

        public ViewModelBase CurrentChildView
        {
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

        // Switch views
        public ICommand ShowLoginViewCommand { get; set; }
        public ICommand ShowRegisterViewCommand { get; set; }


        // Constructor
        public WelcomeViewModel()
        {
            _context = new ToDoDbContext();
            _userRepository = new UserRepository(_context);

            Messenger.Subscribe("ShowLoginView", ExecuteShowLoginViewCommand);
            Messenger.Subscribe("ShowRegisterView", ExecuteShowRegisterViewCommand);

            ShowLoginViewCommand = new ViewModelCommand(ExecuteShowLoginViewCommand);
            ShowRegisterViewCommand = new ViewModelCommand(ExecuteShowRegisterViewCommand);

            visibilityStore = new WelcomePageVisibilityStore();
            visibilityStore.VisibilityChanged += OnVisibilityChanged;

            ExecuteShowLoginViewCommand(null);
        }

        private void OnVisibilityChanged(bool value)
        {
            IsViewVisible = value;
        }

        private void ExecuteShowLoginViewCommand(object obj)
        {
            CurrentChildView = new LoginViewModel(visibilityStore);
        }

        private void ExecuteShowRegisterViewCommand(object obj)
        {
            CurrentChildView = new RegisterViewModel();

        }


    }
}
