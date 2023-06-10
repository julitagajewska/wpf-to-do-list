using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ToDoListApp.MVVM.Model.Converters
{
    internal class PriorityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string cellValue = values[0]?.ToString();
            string priorityLow = values[1]?.ToString();
            string priorityMedium = values[2]?.ToString();
            string priorityHigh = values[3]?.ToString();

            if (cellValue == priorityLow) return "styleLow";
            if (cellValue == priorityMedium) return "styleMedium";
            if (cellValue == priorityHigh) return "styleHigh";

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
