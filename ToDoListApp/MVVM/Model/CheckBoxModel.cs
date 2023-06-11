using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ToDoListApp.MVVM.Model
{
    public class CheckBoxModel
    {
        public Subtask Subtask { get; set; }
        public string ButtonText { get; set; }
        public bool Checked { get; set; }
        // public ICommand ButtonCommand { get; set; }
    }
}
