using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;
using ToDoListApp.MVVM.View;
using ToDoListApp.MVVM.ViewModel;

namespace ToDoListApp
{
    public partial class App : Application
    {
        protected void ApplicationStart(object sender, StartupEventArgs e)
        {
            var welcomeView = new WelcomeView();
            welcomeView.Show();

            welcomeView.IsVisibleChanged += (s, ev) =>
            {
                if(welcomeView.IsVisible == false && welcomeView.IsLoaded)
                {
                    var mainView = new MainView();
                    mainView.Show();
                    //welcomeView.Close();
                }
            };
        }
    }
}
