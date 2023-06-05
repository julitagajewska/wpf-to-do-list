using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListApp.Store
{
    class MainViewStore
    {
        public event Action<string> AddTaskVisibilityChanged;
        public void ChangeAddTaskVisibilty(string isVisible)
        {
            AddTaskVisibilityChanged?.Invoke(isVisible);
        }
    }
}
