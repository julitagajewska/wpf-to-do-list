using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using System.Threading;
using ToDoListApp.Data;
using ToDoListApp.MVVM.Model.Interfaces;
using ToDoListApp.MVVM.Model.Services;

namespace ToDoListApp.MVVM.Model.Converters
{
    public class DateToColorConverter : IValueConverter
    {
        private readonly ToDoDbContext _context;
        private IUserRepository _userRepository;
        public DateToColorConverter()
        {
            _context = new ToDoDbContext();
            _userRepository = new UserRepository(_context);
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime plannerDate)
            {
                var loggedInUser = _userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
                loggedInUser.Planner.CurrentDate = DateTime.Now.Date;
                if (loggedInUser?.Planner?.CurrentDate != null && plannerDate < loggedInUser.Planner.CurrentDate)
                {
                    return new SolidColorBrush(Colors.Red);
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
