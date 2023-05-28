using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;

namespace ToDoListApp.MVVM.ViewModel
{
    public class DetailsTaskViewModel : ViewModelBase
    {
        private MainTask _selectedTask;
        private readonly ToDoDbContext _context;
        public MainTask SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }

        public DetailsTaskViewModel(MainTask selectedTask)
        {
            _context = new ToDoDbContext();
            SelectedTask = _context.MainTasks.Include(t => t.Categories).FirstOrDefault(t => t.Id == selectedTask.Id);
            //SelectedTask = selectedTask;
        }
    }
}
