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
            var dates = Tools.daysSetFromBeginingOfEyar();

            foreach ( var date in dates)
                days.Add(new Day(date));
        }

    }
}
