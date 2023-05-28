using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model.Interfaces;

namespace ToDoListApp.MVVM.Model.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ToDoDbContext _context;
        public CategoryRepository(ToDoDbContext context)
        {
            _context = context;
        }
        public void UpdateCategory(Category category)
        {
            // Sprawdź, czy kategoria jest już dołączona do kontekstu
            if (!_context.Categories.Local.Contains(category))
            {
                // Jeśli nie, dołącz ją do kontekstu
                _context.Categories.Attach(category);
            }

            // Oznacz encję jako zmodyfikowaną
            _context.Entry(category).State = EntityState.Modified;

            // Zapisz zmiany w bazie danych
            _context.SaveChanges();
        }
    }
}
