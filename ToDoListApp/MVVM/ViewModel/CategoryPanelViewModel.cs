using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ToDoListApp.MVVM.ViewModel
{
    public class CategoryPanelViewModel : ViewModelBase
    {
        private readonly ToDoDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private UserModel _loggedInUser;

        private ObservableCollection<Category> _userCategories;
        public ObservableCollection<Category> UserCategories
        {
            get { return _userCategories; }
            set
            {
                _userCategories = value;
                OnPropertyChanged(nameof(UserCategories));
            }
        }

        public ICommand EditCategoryCommand { get; set; }
        public ICommand DeleteCategoryCommand { get; set; }
        public CategoryPanelViewModel(UserModel loggedInUser)
        {
            _context = new ToDoDbContext();
            _loggedInUser = loggedInUser;
            _userRepository = new UserRepository(_context);
            _categoryRepository = new CategoryRepository(_context);
            EditCategoryCommand = new ViewModelCommand(ExecuteEditCategoryCommand);
            DeleteCategoryCommand = new ViewModelCommand(ExecuteDeleteCategoryCommand);

            LoadUserCategories();
        }
        private void LoadUserCategories()
        {
            string username = _loggedInUser.Username;
            UserCategories = _userRepository.GetUserCategories(username);
        }
        private void ExecuteEditCategoryCommand(object obj)
        {
            if(obj is Category selectedCategory)
            {
                // Zapisz zmiany nazwy kategorii do bazy danych
                _categoryRepository.UpdateCategory(selectedCategory);
                _context.SaveChanges();
            }
        }

        private void ExecuteDeleteCategoryCommand(object obj)
        {
            // Logika usuwania kategorii
        }
    }
}
