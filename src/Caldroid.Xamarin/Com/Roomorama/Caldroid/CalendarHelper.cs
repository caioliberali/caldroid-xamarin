using System;
using System.Collections.Generic;
using System.Globalization;
using Java.Text;
using Java.Util;

namespace Caldroid.Xamarin.Com.Roomorama.Caldroid
{
    /// <summary>
    /// Convenient helper to work with date, Date4J DateTime and String
    /// 
    /// @author thomasdao
    /// </summary>
    public class CalendarHelper
    {
        public const string DEFAULT_TIME_FORMAT = "yyyy-MM-dd";


        /// <summary>
        /// Retrieve all the dates for a given calendar month Include previous month,
        /// current month and next month.
        /// </summary>
        /// <returns>The full weeks.</returns>
        /// <param name="year">Year.</param>
        /// <param name="month">Month.</param>
        /// <param name="initialDayOfWeek">Initial day of week.</param>
        /// <param name="maxWeekPerMonth">If set to <c>true</c> max week per month.</param>
        public static List<DateTime> GetFullWeeks(int year, int month, int initialDayOfWeek, bool maxWeekPerMonth)
        {
            var dateTimes = new List<DateTime>();

            var daysInMonth = DateTime.DaysInMonth(year, month);
            var dayOfMonthFirst = new DateTime(year, month, 1);
            var dayOfMonthLast = new DateTime(year, month, daysInMonth);
            var dayOfWeekDayFirst = (int)dayOfMonthFirst.DayOfWeek;

            // If weekday of month is smaller than initial weekday of calendar
            // Example: weekday of month is Monday, weekday of calendar is Tuesday
            // increase the weekday of month because it's in the future.
            if (dayOfWeekDayFirst < initialDayOfWeek)
                dayOfWeekDayFirst = dayOfWeekDayFirst + 7;

            while (dayOfWeekDayFirst > 0)
            {
                var weekdayDiff = dayOfWeekDayFirst - initialDayOfWeek;
                var datePrevMonth = dayOfMonthFirst.AddDays(-weekdayDiff);

                if (datePrevMonth.Date >= dayOfMonthFirst.Date)
                    break;

                dayOfWeekDayFirst = dayOfWeekDayFirst - 1;
                dateTimes.Add(datePrevMonth);
            }

            for (var i = 0; i < dayOfMonthLast.Day; i++)
            {
                dateTimes.Add(dayOfMonthFirst.AddDays(i));
            }

            var dayOfWeekPrevDay = initialDayOfWeek - 1;

            if (dayOfWeekPrevDay < (int)DayOfWeek.Sunday)
                dayOfWeekPrevDay = (int)DayOfWeek.Saturday;

            if ((int)dayOfMonthLast.DayOfWeek != dayOfWeekPrevDay)
            {
                for (var i = 1; ; i++)
                {
                    var dayNextMonth = dayOfMonthLast.AddDays(i);

                    dateTimes.Add(dayNextMonth);

                    if ((int)dayNextMonth.DayOfWeek == dayOfWeekPrevDay)
                        break;
                }
            }

            if (maxWeekPerMonth)
            {
                var dateTimeTotal = dateTimes.Count;
                var row = dateTimeTotal / 7;
                var numberOfDays = (6 - row) * 7;

                var dateTimeLastDay = dateTimes[dateTimeTotal - 1];

                for (var i = 1; i <= numberOfDays; i++)
                {
                    var dayAdditional = dateTimeLastDay.AddDays(i);

                    dateTimes.Add(dayAdditional);
                }
            }

            return dateTimes;
        }


        public static Date ConvertDateTimeToDate(DateTime dateTime)
        {
            var calendar = Java.Util.Calendar.Instance;

            calendar.Clear();
            calendar.Set(dateTime.Year, dateTime.Month - 1, dateTime.Day);

            return calendar.Time;
        }


        public static Date ConvertStringToDate(string date, string dateFormat)
        {
            if (string.IsNullOrWhiteSpace(dateFormat))
                dateFormat = DEFAULT_TIME_FORMAT;

            var simpleDateFormat = new SimpleDateFormat(dateFormat, Locale.Default);

            return simpleDateFormat.Parse(date);
        }


        public static DateTime ConvertDateToDateTime(Date date)
        {
            var dateTimeBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return dateTimeBase.AddMilliseconds(date.Time);
        }


        public static DateTime ConvertStringToDateTime(string dateTime, string dateFormat)
        {
            if (string.IsNullOrWhiteSpace(dateFormat))
                dateFormat = DEFAULT_TIME_FORMAT;

            return DateTime.ParseExact(dateTime, dateFormat, CultureInfo.InvariantCulture);
        }


        public static DateTime FirstDateTimeOfPreviousMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(-1);
        }


        public static DateTime LastDateTimeOfPreviousMonth(DateTime dateTime)
        {
            var day = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

            return new DateTime(dateTime.Year, dateTime.Month, day).AddMonths(-1);
        }


        public static DateTime FirstDateTimeOfNextMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1);
        }


        public static DateTime LastDateTimeOfNextMonth(DateTime dateTime)
        {
            var day = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

            return new DateTime(dateTime.Year, dateTime.Month, day).AddMonths(1);
        }


        public static List<string> ConvertDateTimeToString(List<DateTime> dateTimes)
        {
            var dateTimeStrings = new List<string>();

            for (var i = 0; i < dateTimes.Count; i++)
            {
                dateTimeStrings.Add(dateTimes[i].ToString(DEFAULT_TIME_FORMAT));
            }

            return dateTimeStrings;
        }
    }
}
