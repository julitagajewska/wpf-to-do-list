using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.Model.Converters;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;

namespace ToDoListApp.MVVM.ViewModel
{
    public class EditTaskViewModel : ViewModelBase
    {
        private readonly ToDoDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly MainTaskService _mainTaskService;
        public string CurrentUsername { get; set; }
        private MainTask _selectedTask;
        public MainTask SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }
        //Status and Priority
        private List<string> Statuses;
        public List<string> statuses //list
        {
            get { return new List<string>() { "To Do", "In Progress", "Done" }; }
            set { Statuses = value; }
        }
        private List<string> Priorities;
        public List<string> priorities //list
        {
            get { return new List<string>() { "Low", "Medium", "High" }; }
            set { Priorities = value; }
        }
        public string status { get; set; } //from db
        public string priority { get; set; } //from db
        //Categories
        public ObservableCollection<Category> TaskCategories { get; set; } //List of User Available Categories
        public ObservableCollection<Category> SelectedCategories { get; set; }

        private ObservableCollection<Category> _listBoxSelectedItems;
        public ObservableCollection<Category> ListBoxSelectedItems
        {
            get { return _listBoxSelectedItems; }
            set
            {
                _listBoxSelectedItems = value;
                OnPropertyChanged(nameof(ListBoxSelectedItems));
            }
        }
        public List<int> SelectedCategoriesIds { get; set; }
        public ICommand EditTaskCommand { get; set; }
        public EditTaskViewModel(MainTask selectedTask)
        {
            _context = new ToDoDbContext();
            _mainTaskService = new MainTaskService(_context);
            _userRepository = new UserRepository(_context);
            CurrentUsername = _userRepository.GetCurrentUsername();
            SelectedTask = _context.MainTasks.Include(t => t.Categories).FirstOrDefault(t => t.Id == selectedTask.Id);

            EditTaskCommand = new ViewModelCommand(EditTask);
            status = selectedTask.Status;
            priority = selectedTask.Priority;
            //Categories
            TaskCategories = new ObservableCollection<Category>(_userRepository.GetUserCategories(CurrentUsername)
                .DistinctBy(category => category.Name)
                .ToList());

            SelectedCategories = new ObservableCollection<Category>();

            SelectedCategoriesIds = selectedTask.Categories.Select(c => c.Id).ToList();

            foreach (var category in TaskCategories)
            {
                var isSelected = SelectedCategoriesIds.Contains(category.Id);
                if (isSelected)
                {
                    SelectedCategories.Add(category);
                }
            }
            ListBoxSelectedItems = SelectedCategories;
        }
        private void EditTask(object obj)
        {
            if (SelectedTask != null)
            {
                if (ListBoxSelectedItems.Count == 0)
                {
                    // Wyświetl powiadomienie o konieczności wybrania kategorii
                    MessageBox.Show("You must to select a category/ categories", "No Categories", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                SelectedTask.Categories.Clear();

                foreach (var category in ListBoxSelectedItems)
                {
                    SelectedTask.Categories.Add(category);
                }

                _mainTaskService.UpdateMainTask(SelectedTask);
                _context.SaveChanges();
                SelectedTask = null;
                Messenger.Publish("ShowAllTasksView");
            }
        }
    }
}
