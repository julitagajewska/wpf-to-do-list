using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ToDoListApp.MVVM.ViewModel
{
    public class CategoryPanelViewModel : ViewModelBase, IDataErrorInfo
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
                OnPropertyChanged(nameof(ValidationString));
            }
        }
        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                OnPropertyChanged(nameof(ValidationString));

            }
        }
        //Add New Category
        private bool _isAddCategoryChecked;
        public bool IsAddCategoryChecked
        {
            get { return _isAddCategoryChecked; }
            set
            {
                _isAddCategoryChecked = value;
                OnPropertyChanged(nameof(IsAddCategoryChecked));
                OnPropertyChanged(nameof(ValidationString));
                if (!value)
                {
                    // Jeśli przycisk został odznaczony, dodaj nową kategorię na liście
                    if (!string.IsNullOrEmpty(NewCategoryName))
                    {
                        UserCategories.Add(new Category { Name = NewCategoryName, IsCustom = true });
                        NewCategoryName = string.Empty; // Wyczyść pole tekstowe
                    }
                }
            }
        }
        //Add new Category
        private string _newCategoryName;
        public string NewCategoryName
        {
            get { return _newCategoryName; }
            set
            {
                _newCategoryName = value;
                OnPropertyChanged(nameof(NewCategoryName));
                OnPropertyChanged(nameof(ValidationString));
            }
        }
        public ICommand AddCategoryCommand { get; set; }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
                OnPropertyChanged(nameof(IsAddCategoryVisible));
                OnPropertyChanged(nameof(NewCategoryNameEnabled));
            }
        }

        public bool IsAddCategoryVisible => !IsEditMode;
        public bool NewCategoryNameEnabled => !IsEditMode || IsAddCategoryChecked;

        //Walidacja
        public string this[string columnName]
        {
            get
            {
                if (SelectedCategory != null)
                {

                    if (string.IsNullOrEmpty(SelectedCategory.Name))
                    {
                       return "Category Name is required.";
                    }
                   
                }
                return null;
            }   
        
        }
        public string Error => throw new NotImplementedException();
        public string ValidationString
        {
            get
            {
                // Sprawdź, czy istnieją jakieś błędy walidacji dla poszczególnych właściwości
                var propertyErrors = typeof(CreateTaskViewModel)
                    .GetProperties()
                    .Where(p => this[p.Name] != null)
                    .Select(p => this[p.Name]);

                // Sprawdź, czy istnieją błędy walidacji dla innych warunków
                var otherErrors = GetOtherValidationErrors(); // Zaimplementuj tę metodę, aby zwracała inne błędy walidacji

                // Połącz wszystkie błędy walidacji w jeden ciąg tekstowy
                var validationErrors = propertyErrors.Concat(otherErrors);
                return string.Join(Environment.NewLine, validationErrors);
            }
        }
        private IEnumerable<string> GetOtherValidationErrors()
        {
            List<string> errors = new List<string>();

            if (SelectedCategory != null)
            {
                ObservableCollection<Category> userCategories = _userRepository.GetUserCategories(_userRepository.GetCurrentUsername());
                bool categoryExists = userCategories.Any(category => category.Name.Equals(SelectedCategory.Name) && category != SelectedCategory);

                if (categoryExists)
                {
                    errors.Add("Category with the same name already exists.");
                }
            }
            return errors;
        }

        public ICommand EditCategoryCommand { get; set; }
        public ICommand DeleteCategoryCommand { get; set; }
        public ICommand SaveCategoryCommand { get; set; }
        public CategoryPanelViewModel(UserModel loggedInUser)
        {
            _context = new ToDoDbContext();
            _loggedInUser = loggedInUser;
            _userRepository = new UserRepository(_context);
            _categoryRepository = new CategoryRepository(_context);
            EditCategoryCommand = new ViewModelCommand(ExecuteEditCategoryCommand);
            DeleteCategoryCommand = new ViewModelCommand(ExecuteDeleteCategoryCommand);
            SaveCategoryCommand = new ViewModelCommand(ExecuteSaveCategoryCommand, CanExecuteSaveCategoryCommand);
            AddCategoryCommand = new ViewModelCommand(ExecuteAddCategoryCommand);
            LoadUserCategories();
        }
        private void ExecuteAddCategoryCommand(object obj)
        {
            bool categoryExists = UserCategories.Any(category =>
            category.Name.Equals(NewCategoryName, StringComparison.OrdinalIgnoreCase) &&
            category.IsCustom);

            if (!string.IsNullOrEmpty(NewCategoryName) && !categoryExists)
            {
                var category = new Category
                {
                    Name = NewCategoryName,
                    IsCustom = true,
                    Owner = _loggedInUser.Id // Przypisanie id Usera.
                };
                var otherValidationErrors = GetOtherValidationErrors();
                if (otherValidationErrors.Any())
                {
                    return;
                }
                UserCategories.Add(category);
                _context.Categories.Add(category);
                _context.SaveChanges();
                NewCategoryName = string.Empty;
                
            }
        }
        private void LoadUserCategories()
        {
            string username = _loggedInUser.Username;
            UserCategories = _userRepository.GetUserCategories(username);
        }
        private void ExecuteEditCategoryCommand(object obj)
        {
            IsEditMode = true;

            if (obj is Category selectedCategory)
            {
                
                SelectedCategory = selectedCategory;
            }
            
        }

        private void ExecuteSaveCategoryCommand(object obj)
        {
            if (!string.IsNullOrEmpty(ValidationString))
            {
                // Jeśli istnieją błędy walidacyjne, przerwij wykonanie funkcji
                return;
            }
            if (SelectedCategory != null)
            {
                // Zapisz zmiany nazwy kategorii do bazy danych
                _categoryRepository.UpdateCategory(SelectedCategory);
                _context.SaveChanges();
                SelectedCategory = null;
                IsEditMode = false;
                
            }
            
        }

        private bool CanExecuteSaveCategoryCommand(object obj)
        {
            return SelectedCategory != null;
        }

        private void ExecuteDeleteCategoryCommand(object obj)
        {
            if (obj is Category categoryToDelete)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this category?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _categoryRepository.DeleteCategory(categoryToDelete);
                    _context.SaveChanges();
                }
            }
            LoadUserCategories();
        }
    }
}