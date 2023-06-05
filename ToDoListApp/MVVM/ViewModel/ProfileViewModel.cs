using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model;
using System.Threading;
using ToDoListApp.MVVM.Model.Services;

namespace ToDoListApp.MVVM.ViewModel
{
    public class ProfileViewModel : ViewModelBase
    {
        private UserAccountModel _currentUserAccount;
        private readonly ToDoDbContext _context;
        private IUserRepository _userRepository;

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

        public ProfileViewModel()
        {
            _context = new ToDoDbContext(); // Create an instance of ToDoDbContext
            _userRepository = new UserRepository(_context); // Pass the ToDoDbContext instance to the UserRepository constructor
            CurrentUserAccount = new UserAccountModel();

            LoadCurrentUserData();
            username = CurrentUserAccount.Username;
        }

        private void LoadCurrentUserData()
        {
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
