using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{
    internal static class Manager
    {
        public static Objective root = new Objective("1");

        public static Objective today = new Objective("1");
        public static List<string> tasks_for_today;


        public static void initialize()
        {
            SQLite.initialize();
        }

        public static void execute()
        {
            root.form();
            set_today_tasks();
        }

        private static void set_today_tasks()
        {
            tasks_for_today = SQLite.get_tasks_for_today();
            root.check_for_today();
            root.check_for_today_back();

            today = root;
        }

    }
}
