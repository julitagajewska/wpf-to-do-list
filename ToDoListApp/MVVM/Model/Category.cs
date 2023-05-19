using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListApp.MVVM.Model
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public ICollection<MainTask>? MainTasks { get; set; }

    }
}
