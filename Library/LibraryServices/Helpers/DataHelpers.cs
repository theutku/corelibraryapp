using System;
using System.Collections.Generic;
using LibraryData.Models;

namespace LibraryServices.Helpers
{
    public class DataHelpers
    {
        public static IEnumerable<string> HumanizeBusinessHours(IEnumerable<BranchHours> branchHours)
        {
            List<string> hours = new List<string>();

            foreach (BranchHours time in branchHours)
            {
                string day = HumanizeDay(time.DayOfWeek);
                string openTime = HumanizeTime(time.OpenTime);
                string closeTime = HumanizeTime(time.CloseTime);

                string timeEntry = $"{day} {openTime} to {closeTime}";
                hours.Add(timeEntry);
            }

            return hours;
        }

        private static string HumanizeDay(int dayOfWeek)
        {
            return Enum.GetName(typeof(DayOfWeek), dayOfWeek);
        }

        private static string HumanizeTime(int time)
        {
            return TimeSpan.FromHours(time).ToString("hh':'mm");
        }

    }
}
