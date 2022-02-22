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
        public static ObservableCollection<Objective> projects { get; set; } = new ObservableCollection<Objective>();

        public static void initialize()
        {
            //Log.initialize();
            DBS.initialize();
            //DBInteraction.initialize();
        }

        public static void check()
        {

        }

        public static void execute()
        {
            check();
            connect_to_db();

            form_projects();
        }


        private static void form_projects()
        {
            projects = DBInteraction.get_projects();

            foreach (Objective obj in projects)
                obj.form_subtasks();
        }

        // db

        public static void connect_to_db()
        {
            DBS.open();
        }

        public static void disconnect_to_db()
        {
            DBS.close();
        }

    }
}
