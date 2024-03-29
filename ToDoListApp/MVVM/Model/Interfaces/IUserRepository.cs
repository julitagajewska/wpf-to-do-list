﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListApp.MVVM.Model.Interfaces
{
    public interface IUserRepository
    {
        bool AuthenticateUser(NetworkCredential credential);
        void Add(UserModel userModel);
        void Edit(UserModel userModel);
        void Delete(UserModel userModel);
        UserModel GetById(int id);
        UserModel GetByUsername(string username);
        IEnumerable<UserModel> GetAlll();
        string GetCurrentUsername();
        ObservableCollection<Category> GetUserCategories(UserModel userModel);
        Planner GetPlannerByUsername(string username);
        // ...
    }
}
