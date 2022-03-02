using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{
    internal static class StoredProcedures
    {
        static public readonly string get_projects = "get_projects";
        static public readonly string get_tasks = "get_tasks";
        static public readonly string add_task = "add_task";
        static public readonly string delete_task = "delete_task";
        static public readonly string add_task_to_today = "add_task_to_today";
        static public readonly string delete_task_from_today = "delete_task_from_today";
        static public readonly string set_done = "set_done";
        static public readonly string set_name = "set_name";
        static public readonly string set_description = "set_description";
    }
}
