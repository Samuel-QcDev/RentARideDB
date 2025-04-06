using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentARideDB.Tools
{
    public class CarOptionsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Log the value received for debugging
            System.Diagnostics.Debug.WriteLine($"Value received in converter: {value} ({value?.GetType().Name})");

            // Attempt to cast the value to a List<string>
            var carOptions = value as List<string>;

            //System.Diagnostics.Debug.WriteLine($"Car options: {string.Join(", ", )}");
            // Check if the carOptions list is not null and contains items
            if (carOptions != null && carOptions.Any())
            {
                // Log the contents of the list
                System.Diagnostics.Debug.WriteLine($"Car options: {string.Join(", ", carOptions)}");

                // Return a string with the contents of the list, separated by commas
                return string.Join(", ", carOptions);
            }

            // If the list is null or empty, return a default message
            return "No options";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; // We don't need to handle ConvertBack here
        }
    }
}
