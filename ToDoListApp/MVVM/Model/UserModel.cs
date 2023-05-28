using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListApp.MVVM.Model
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        [ForeignKey("Planner")]
        public int PlannerId { get; set; }
        public Planner Planner { get; set; }
        public UserModel()
        {
            Planner = new Planner();
        }

    }
}
