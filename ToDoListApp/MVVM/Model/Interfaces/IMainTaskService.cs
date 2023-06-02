using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.MVVM.Model.Converters;

namespace ToDoListApp.MVVM.Model.Interfaces
{
    public interface IMainTaskService
    {
        void CreateMainTask(MainTask task);
        void UpdateMainTask(MainTask task);
        void DeleteTask(MainTask task);
        string ConvertCategoriesToString(ICollection<Category> categories);
    }
}
