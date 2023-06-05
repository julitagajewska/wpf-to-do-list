using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model;

namespace ToDoListApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class WelcomeView : Window
    {
        public WelcomeView()
        {
            InitializeComponent();
            /*var planner = new Planner();
            UserModel user = new UserModel
            {
                Id = "1",
                Username = "admin",
                Password = "123",
                Email = "admin@test.com",
                Planner = planner
            };
            using (var dbContext = new ToDoDbContext())
            {
                dbContext.Users.Add(user);
                dbContext.Planners.Add(planner);
                dbContext.SaveChanges();
            }*/
            //User2
            /*var planner = new Planner();
            UserModel user = new UserModel
            {
                Id = "2",
                Username = "ola",
                Password = "ola",
                Email = "ola@test.com",
                Planner = planner
            };
            using (var dbContext = new ToDoDbContext())
            {
                dbContext.Users.Add(user);
                dbContext.Planners.Add(planner);
                dbContext.SaveChanges();
            }*/
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton==MouseButtonState.Pressed) // Move the window around
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
             
        }
    }
}
