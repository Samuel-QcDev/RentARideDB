using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentARideDB.Models
{
 
public class RadioButtonValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string selectedValue = value as string;
            string buttonValue = parameter as string;

            return selectedValue == buttonValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value; // This is one-way binding, no need to handle ConvertBack
        }
    }
}
