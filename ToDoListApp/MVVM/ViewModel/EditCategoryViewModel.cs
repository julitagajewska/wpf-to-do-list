using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ToDoListApp.MVVM.ViewModel
{
    public class EditCategoryViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly ToDoDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly CategoryRepository _categoryRepository;

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

        //Walidacja
        public string this[string columnName]
        {
            get
            {
                if (SelectedCategory != null)
                {
                    if (string.IsNullOrEmpty(SelectedCategory.Name))
                    {
                        return "Category Name is required";
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
                var propertyErrors = typeof(EditCategoryViewModel)
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
            var user = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            ObservableCollection<Category> userCategories = _userRepository.GetUserCategories(user);
            bool categoryExists = userCategories.Any(category =>
            category.Name.Equals(SelectedCategory.Name, StringComparison.OrdinalIgnoreCase)
            && category!=SelectedCategory);

            if (categoryExists)
            {
                errors.Add("Category with the same name already exists.");
            }
            return errors;
        }
        //użytkownik
        private UserModel _loggedInUser;
        private string CurrentUsername { get; set; }
        public ICommand EditCategoryCommand { get; set; }
        public EditCategoryViewModel(Category selectedCategory)
        {
            _context = new ToDoDbContext();
            _categoryRepository = new CategoryRepository(_context);
            _userRepository = new UserRepository(_context);
            CurrentUsername = _userRepository.GetCurrentUsername();
            _loggedInUser = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            SelectedCategory =_context.Categories.FirstOrDefault(t => t.Id == selectedCategory.Id);

            EditCategoryCommand= new ViewModelCommand(EditCategory);
        }
        private void EditCategory(object obj)
        {
            if (SelectedCategory != null)
            {
                OnPropertyChanged(nameof(ValidationString));
                if (!string.IsNullOrEmpty(ValidationString))
                {
                    return;
                }
                _categoryRepository.UpdateCategory(SelectedCategory);
                _context.SaveChanges();
                Messenger.Publish("ShowCategoryPanelView");
            }
        }
    }
}
