using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
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
            //tutaj będzie rejetracja, mniej więcej w taki sposób.
            /*var user = new User
            {
                Username = Username,
                // inne pola użytkownika
            };

            var planner = new Planner();
            user.Planner = planner;

            _context.Users.Add(user);
            _context.Planners.Add(planner);
            _context.SaveChanges();*/
            throw new NotImplementedException();
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
    }
}
