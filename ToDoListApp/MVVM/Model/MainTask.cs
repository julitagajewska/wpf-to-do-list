using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListApp.MVVM.Model
{
    public class MainTask
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? StartDate { get; set;  }
        public DateTime? EndDate { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string Priority { get; set; }
        [ForeignKey("Planner")]
        public int PlannerId { get; set; }
        public Planner Planner { get; set; }

        public ICollection<Subtask>? Subtasks { get; set; }

        public ICollection<Category> Categories { get; set; }


    }
}
