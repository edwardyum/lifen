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
        public static Objective root { get; set; }
        public static Objective today { get; set; } 
        public static List<string> tasks_for_today;

        public static event Refresh refresh;

        public static void initialize()
        {
            SQLite.initialize();
        }

        public static bool event_contains_method(Type for_check)    // почему-то не срабатывает обновление и обовляется только в некоторых случаях при k=2
        {
            int k = 0;
            if (refresh != null)
            {
                Delegate[] delegates = refresh.GetInvocationList();

                foreach (Delegate d in delegates)
                {
                    Type dt = d.Method.DeclaringType;
                    if (dt.Equals(for_check))
                        k++;
                    
                    if(k>=2)
                        return true;
                }
            }
                
            return false;
        }

        public static void execute()
        {
            form();
            set_today_tasks();

            refresh.Invoke();
        }

        private static void form()
        {
            root = null;
            root = new Objective("1");

            root.form();
        }

        private static void set_today_tasks()
        {
            tasks_for_today = SQLite.get_tasks_for_today();
            root.check_for_today();
            root.check_for_today_back();

            today = new Objective("1");
            root.create_today_only_structure_2(today);
        }

    }
}
