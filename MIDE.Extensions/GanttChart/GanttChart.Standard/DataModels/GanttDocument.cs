using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanttChart.DataModels
{
    public class GanttDocument
    {
        public Day[] DefaultWorkload { get; }
        public List<Year> Years { get; }

        public GanttDocument()
        {
            Years = new List<Year>();
            DefaultWorkload = Day.GetDays(null, 7, 0, 7);
        }

        public void AddYear(int year)
        {

        }
        public void Resize(TimeSpan timeSpan)
        {

        }
    }

    public class Year
    {
        public int Index { get; }
        public List<Month> Months { get; }
        public Day[] DefaultWorkloads { get; }

        public Year(int index)
        {
            Months = new List<Month>();
            Index = index;
            DefaultWorkloads = Day.GetDays(null, 7, 1, 7);
        }

        public void AddMonth(int month)
        {
            if (month < 1 || month > 12)
                throw new IndexOutOfRangeException();
            Months.Add(new Month(month, this, DefaultWorkloads));
        }
    }
    public class Month
    {
        public int Index { get; }
        public int DaysCount { get; private set; }
        public Year Year { get; }
        public Day[] DefaultWorkload { get; }
        public List<Week> Weeks { get; }

        public Month(int index, Year year, byte workload)
        {
            Year = year;
            Index = index;
            Weeks = new List<Week>();
            DefaultWorkload = Day.GetDays(null, workload, 0, 7);
        }
        public Month(int index, Year year, Day[] defaultWorkloads)
        {
            Year = year;
            Index = index;
            Weeks = new List<Week>();
            DefaultWorkload = Day.GetDays(null, defaultWorkloads);
        }

        private void PopulateWeeks()
        {
            DateTime reference = new DateTime(2019, 6, 1);
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
            Calendar calendar = ci.Calendar;

            IEnumerable<int> daysInMonth = Enumerable.Range(1, calendar.GetDaysInMonth(reference.Year, reference.Month));

            List<(DateTime, DateTime)> weeks = daysInMonth.Select(day => new DateTime(reference.Year, reference.Month, day))
                .GroupBy(d => calendar.GetWeekOfYear(d, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                .Select(g => (g.First(), g.Last()))
                .ToList();
        }
    }
    public class Week
    {
        public int TotalWorkload => Days.Sum(d => d.Workload);
        public int Index { get; set; }
        public Month Month { get; }
        public Day[] Days { get; }

        public Week(Month month, int index, byte workload)
        {
            Index = index;
            Month = month;
            Days = Day.GetDays(this, workload, 0, 7);
        }
        public Week(Month month, Day[] defaultWorkloads)
        {
            Month = month;
            Days = Day.GetDays(this, defaultWorkloads);
        }
    }
    public class Day
    {
        private byte workload;

        public byte Workload
        {
            get => workload;
            set
            {
                if (value > 24)
                    return;
                workload = value;
            }
        }
        public int Index { get; }
        public Week Week { get; }

        public Day(int index, Week week, byte workload = 0)
        {
            Index = index;
            Week = week;
            Workload = workload;
        }

        public static Day Sunday(Week week, byte workload) => new Day(0, week, workload);
        public static Day Monday(Week week, byte workload) => new Day(1, week, workload);
        public static Day Tuesday(Week week, byte workload) => new Day(2, week, workload);
        public static Day Wednesday(Week week, byte workload) => new Day(3, week, workload);
        public static Day Thursday(Week week, byte workload) => new Day(4, week, workload);
        public static Day Friday(Week week, byte workload) => new Day(5, week, workload);
        public static Day Saturday(Week week, byte workload) => new Day(6, week, workload);
        public static Day[] GetDays(Week week, byte workload, int from, int count)
        {
            Day[] days = new Day[count];
            for (int i = from; i < count; i++)
            {
                days[i] = new Day(i, week, workload);
            }
            return days;
        }
        public static Day[] GetDays(Week week, Day[] defaultWorkloads)
        {
            Day[] days = new Day[defaultWorkloads.Length];
            for (int i = 0; i < days.Length; i++)
            {
                days[i] = new Day(defaultWorkloads[i].Index, week, defaultWorkloads[i].workload);
            }
            return days;
        }
    }
}