using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model.Interfaces;

namespace ToDoListApp.MVVM.Model.Services
{
    public class MainTaskService : IMainTaskService
    {
        private readonly ToDoDbContext _context;
        public MainTaskService(ToDoDbContext context)
        {
            _context = context;
        }
        public void CreateMainTask(MainTask task)
        {
            // Logika tworzenia zadania w bazie danych
            _context.MainTasks.Add(task);
            _context.SaveChanges();
        }
        public string ConvertCategoriesToString(ICollection<Category> categories)
        {
            return string.Join(", ", categories.Select(c => c.Name));
        }
    }
}
