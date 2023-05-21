using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListApp.MVVM.Model.Interfaces
{
    public interface IMainTaskService
    {
        void CreateMainTask(MainTask task);
    }
}
