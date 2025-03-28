using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentARideDB.Tools
{
    public class Utils
    {
        public static DateTime RoundToNearest30Minutes(DateTime dateTime)
        {
            // Calculate the total minutes from the start of the day
            int totalMinutes = (dateTime.Hour * 60) + dateTime.Minute;

            // Round the total minutes to the nearest 30
            int roundedMinutes = (int)(Math.Round((double)totalMinutes / 30) * 30);

            // Convert back to hours and minutes
            int roundedHours = roundedMinutes / 60;
            int finalMinutes = roundedMinutes % 60;

            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, roundedHours, finalMinutes, 0);
        }
    }
}
