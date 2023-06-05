using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model.Converters;
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
            _context.MainTasks.Add(task);
            _context.SaveChanges();
        }
        public void UpdateMainTask(MainTask task)
        {
            // Sprawdź, czy kategoria jest już dołączona do kontekstu
            var existingCategoryIds = _context.MainTasks
                .Where(mt => mt.Id == task.Id)
                .SelectMany(mt => mt.Categories)
                .Select(c => c.Id)
                .ToList();

            var categoriesToAdd = task.Categories.Where(c => !existingCategoryIds.Contains(c.Id)).ToList();

            foreach (var category in categoriesToAdd)
            {
                // Jeśli kategoria nie jest dołączona do kontekstu, dołącz ją
                if (!_context.Categories.Local.Any(c => c.Id == category.Id))
                {
                    _context.Categories.Attach(category);
                }
            }

            // Oznacz encję jako zmodyfikowaną
            _context.Entry(task).State = EntityState.Modified;

            _context.SaveChanges();
        }
        public void DeleteTask(MainTask task)
        {

            _context.MainTasks.Remove(task);
            _context.SaveChanges();
        }
        public string ConvertCategoriesToString(ICollection<Category> categories)
        {
            return string.Join(", ", categories.Select(c => c.Name));
        }

    }
}
