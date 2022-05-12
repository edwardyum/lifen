using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{
    internal class DiaryViewModel
    {
        public ObservableCollection <Day> days { get; set; } = new ObservableCollection<Day>();

        public DiaryViewModel()
        {
            form();
        }

        private void form()
        {
            int month = DateTime.Now.Month-1;
            int year = DateTime.Now.Year;

            int ndays = DateTime.DaysInMonth(year, month);

            for (int i = 0; i < ndays; i++)
            {
                string date = $"{(i + 1)}.{month}.{year}";
                days.Add(new Day(DateTime.Parse(date)));
            }
        }
    }
}
