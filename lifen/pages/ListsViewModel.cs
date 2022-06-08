using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Reflection;
using Windows.UI.Xaml;

namespace lifen
{
    internal class ListsViewModel : DependencyObject
    {
        public static bool forming = false;

        public Objective root { get { return (Objective)GetValue(rootProperty); } set { SetValue(rootProperty, value); } }
        public static readonly DependencyProperty rootProperty = DependencyProperty.Register("root", typeof(Objective), typeof(ListsViewModel), new PropertyMetadata(null));

        public Objective today { get { return (Objective)GetValue(todayProperty); } set { SetValue(todayProperty, value); } }
        public static readonly DependencyProperty todayProperty = DependencyProperty.Register("today", typeof(Objective), typeof(ListsViewModel), new PropertyMetadata(null));

        public static List<string> todayTasks;

        public ListsViewModel()
        {
            root = new Objective("1", null);  // корневой узел

            checkToday();
            today = new Objective(root.Id, root.Name, root.Description, root.Done);
            root.create_today_only_structure_2(today);
        }

        private void checkToday()
        {
            todayTasks = SQLite.get_column(Tables.planner, Planner.task, Planner.date, Time.now_date());

            root.check_for_today();
            root.check_for_today_back();
        }

        //private List<string> getTodayTasks()
        //{
        //    return SQLite.get_column(Tables.planner, Planner.task, Planner.date, Time.now_date());
        //}


        //private void refresh_data()
        //{
        //    root = null;
        //    root = Manager.root;

        //    today = null;
        //    today = Manager.today;


        //}


    }
}
