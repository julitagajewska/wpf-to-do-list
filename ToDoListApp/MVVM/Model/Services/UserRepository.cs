using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model.Interfaces;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace ToDoListApp.MVVM.Model.Services
{
    public class UserRepository : IUserRepository//RepositoryBase, 
    {
        private readonly ToDoDbContext _context;
        public UserRepository(ToDoDbContext context)
        {
            _context = context;
        }
        public void Add(UserModel userModel)
        {
            //pakiet kategorii
            Category school=new Category{ Name="School", IsCustom=false,Owner= userModel.Id};
            Category home=new Category { Name = "Home", IsCustom = false, Owner = userModel.Id };
            Category work=new Category { Name = "Work", IsCustom = false, Owner = userModel.Id };
            _context.Categories.AddRange(school, home, work);
            _context.Users.Add(userModel); 
            _context.Planners.Add(userModel.Planner);
            _context.SaveChanges();

        }

        public bool AuthenticateUser(NetworkCredential credential)
        {
            bool validUser = _context.Users.Any(user =>
                user.Username == credential.UserName && user.Password == credential.Password);

            return validUser;
        }

        public void Delete(UserModel userModel)
        {
            throw new NotImplementedException();
        }

        public void Edit(UserModel userModel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserModel> GetAlll()
        {
            throw new NotImplementedException();
        }

        public UserModel GetById(int id)
        {
            throw new NotImplementedException();
        }
        public string GetCurrentUsername()
        {
            IPrincipal user = Thread.CurrentPrincipal;
            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                return user.Identity.Name;
            }

            return null;
        }
        public UserModel GetByUsername(string username)
        {
            UserModel user = _context.Users.FirstOrDefault(u => u.Username == username);
            return user;
        }
        public Planner GetPlannerByUsername(string username)
        {
            UserModel user = GetByUsername(username);

            if (user != null)
            {
                if (user.PlannerId != null)
                {
                    return _context.Planners.FirstOrDefault(planner => planner.Id == user.PlannerId);
                }
            }
            return null;
        }

        public ObservableCollection<Category> GetUserCategories(UserModel user)
        {
            //UserModel user = GetByUsername(username);
            if (user != null && user.Planner != null)
            {
                List<Category> userCategories = _context.MainTasks //Kategorie, które są przypisane do tasków
                    .Include(task => task.Categories)
                    .Where(task => task.PlannerId == user.PlannerId)
                    .SelectMany(task => task.Categories)
                    .Where(category => category.Owner == user.Id) // Dodatkowe filtrowanie po OwnerId
                    .Distinct()
                    .ToList();
                //customowe kategorie + te, które nie mają tasków
                List<Category> customCategories = _context.Categories
                    .Where(category => (!category.IsCustom && category.Owner == user.Id) ||
                               (category.MainTasks.Count == 0 && category.Owner == user.Id))
                    .Distinct()
                    .ToList();

                List<Category> allCategories = userCategories.Concat(customCategories).Distinct().ToList();

                return new ObservableCollection<Category>(allCategories);
            }
            return new ObservableCollection<Category>();
        }
    }
}
