using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListApp.MVVM.Model
{
    public class Subtask
    {
        [Key]
        public int Id { get; set; }

        public Task Task { get; set; }

    }
}
