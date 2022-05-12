using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace lifen
{
    internal class Day
    {
        public DateTime Date { get; set; }
        public bool first_day_of_month = false;
        public BitmapImage image { get; set; }
        public string lable { get; set; }

        public ObservableCollection<string> done { get; set; }


        public Day(DateTime Date)
        {
            this.Date = Date;
            form();
        }

        public void form()
        {
            // список задач за этот день
            List<string> list = SQLite.contains(Tables.tasks,Tasks.name,Tasks.completion_date, Date.ToString("d"));
            done = new ObservableCollection<string>(list);
        }
    }
}
