using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;
using System.Windows.Input;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;

namespace ToDoListApp.MVVM.ViewModel
{
    public class DetailsTaskViewModel : ViewModelBase
    {
        private MainTask _selectedTask;
        private readonly ToDoDbContext _context;
        private readonly IMainTaskService _mainTaskService;
        private string _caption;
        private ObservableCollection<CheckBoxModel> _checkboxes;
        public ObservableCollection<CheckBoxModel> Checkboxes
        {
            get { return _checkboxes; }
            set
            {
                _checkboxes = value;
                OnPropertyChanged(nameof(Checkboxes));
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
        public MainTask SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }
        public ICommand EditTasksViewCommand { get; set; }
        public ICommand DeleteTasksViewCommand { get; set; }
        public ICommand ToggleStatusCommand { get; set; }

        public DetailsTaskViewModel(MainTask selectedTask)
        {
            _context = new ToDoDbContext();
            _mainTaskService = new MainTaskService(_context);
            SelectedTask = _context.MainTasks.Include(t => t.Categories).Include(t => t.Subtasks).FirstOrDefault(t => t.Id == selectedTask.Id);

            _checkboxes = new ObservableCollection<CheckBoxModel>();
            foreach (Subtask task in SelectedTask.Subtasks)
            {
                bool status = false;
                if (task.Status == "Done") status = true;
                _checkboxes.Add(new CheckBoxModel { Subtask = task, ButtonText = task.Name, Checked = status });
            }

            EditTasksViewCommand = new ViewModelCommand(ShowEditTaskViewCommand);
            DeleteTasksViewCommand = new ViewModelCommand(ExecuteDeleteTaskCommand);
            ToggleStatusCommand = new ViewModelCommand(ExecuteToggleStatusCommand);
        }
        private void ShowEditTaskViewCommand(object obj)
        {
            if (SelectedTask!=null)
            {
                Messenger.Publish("ShowEditTasksView", SelectedTask);
                Caption = "Edit Task";
            }
        }
        private void ExecuteDeleteTaskCommand(object obj)
        {
            if (obj is MainTask taskToDelete)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this Task?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _mainTaskService.DeleteTask(taskToDelete);
                    _context.SaveChanges();
                    Messenger.Publish("ShowAllTasksView");
                }
            }
        }

        private void ExecuteToggleStatusCommand(object obj)
        {
            Subtask subtask = (Subtask)obj;
            if (subtask.Status == "To Do") {
                subtask.Status = "Done";
            }
            else
            {
                subtask.Status = "To Do";
            }
            _mainTaskService.UpdateMainTask(SelectedTask);
            _context.SaveChanges();

            //bool isTaskFinished = true;

            //foreach(Subtask task in SelectedTask.Subtasks)
            //{
            //    if(task.Status != "Done") isTaskFinished = false;
            //}

            //if (isTaskFinished)
            //{
            //    SelectedTask.Status = "Done";
            //}

            //_mainTaskService.UpdateMainTask(SelectedTask);
            //_context.SaveChanges();
        }
    }
}
