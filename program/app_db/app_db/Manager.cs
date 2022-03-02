using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_db
{
    internal static class Manager
    {
        public static Objective objective = new Objective("1");
        public static List<string> tasks_for_today;

        public static void execute()
        {
            objective.form();

            tasks_for_today = SQLite.get_tasks_for_today();
            objective.check_for_today();
            objective.check_for_today_back();
        }


    }
}
