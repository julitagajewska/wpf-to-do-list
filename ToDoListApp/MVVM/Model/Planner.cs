using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListApp.MVVM.Model
{
    public class Planner
    {
        [Key]
        public int Id { get; set; }
        public UserModel User { get; set; }

        public DateTime CurrentDate { get; set; }

        public ICollection<MainTask>? MainTasks { get; set; }
        public Planner()
        {
            MainTasks = new List<MainTask>(); // Dodaj inicjalizację kolekcji MainTasks
        }

    }
}
