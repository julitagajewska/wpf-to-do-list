﻿using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public void AddCategoryToUser(string username, string newCategoryName)
        {
            UserModel user = GetByUsername(username);
            Planner planner = GetPlannerByUsername(username);
            if (user != null && planner != null)
            {
                planner.MainTasks = _context.MainTasks
                .Where(task => task.PlannerId == planner.Id)
                .ToList();
                 //Sprawdź, czy kategoria już istnieje w plannerze użytkownika
                bool categoryExists = user.Planner.MainTasks
                    .Where(t => t.PlannerId == user.PlannerId)
                    .SelectMany(t => t.Categories)
                    .Any(cat => cat.Name.Equals(newCategoryName, StringComparison.OrdinalIgnoreCase));



                if (!categoryExists)
                {
                    var category = new Category
                    {
                        Name = newCategoryName,
                        IsCustom = true
                    };

                    user.Planner.MainTasks = user.Planner.MainTasks ?? new List<MainTask>();
                    foreach (var task in user.Planner.MainTasks)
                    {
                        task.Categories = task.Categories ?? new List<Category>();
                        task.Categories.Add(category);
                    }

                    _context.Categories.Add(category);
                    _context.SaveChanges();
                }
            }
        }
        public ObservableCollection<Category> GetUserCategories(string username)
        {
            UserModel user = GetByUsername(username);
            if (user != null && user.Planner != null)
            {
                List<Category> userCategories = _context.MainTasks
                    .Where(task => task.PlannerId == user.PlannerId)
                    .SelectMany(task => task.Categories)
                    .Distinct()
                    .ToList();

                List<Category> customCategories = _context.Categories
                    .Where(category => !category.IsCustom)
                    .ToList();

                List<Category> allCategories = userCategories.Concat(customCategories).ToList();

                return new ObservableCollection<Category>(allCategories);
            }
            return new ObservableCollection<Category>();
        }
    }
}