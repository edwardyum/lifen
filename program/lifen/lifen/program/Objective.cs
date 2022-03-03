using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{
    internal class Objective
    {
        //
        private string id = string.Empty;

        //
        private string name = string.Empty;
        private string description = string.Empty;
        private bool done = false;
        private string data_creation = string.Empty;
        private string data_completion = string.Empty;

        public bool added_for_today = false;

        //
        public bool obtaining_data_from_db = false;
        public string Name { get { return name; } set { name = value; if (!obtaining_data_from_db) { } } }
        public bool Done { get { return done; } set { done = value; if (!obtaining_data_from_db) { } } }
        public string Description { get { return description; } set { description = value; if (!obtaining_data_from_db) { } } }
        public string DataCreation { get { return data_creation; } set { data_creation = value; if (!obtaining_data_from_db) { } } }
        public string DataCompletion { get { return data_completion; } set { data_completion = value; if (!obtaining_data_from_db) { } } }


        public ObservableCollection<Objective> subtasks { get; set; } = new ObservableCollection<Objective>();


        public Objective(string Id)
        {
            id = Id;
        }

        private Dictionary<string, string> this_task()
        {
            Dictionary<string, string> where = new Dictionary<string, string>();
            where.Add(Tasks.Id, id);
            return where;
        }


        public void add_task()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add(Tasks.name, "задача");
            values.Add(Tasks.creation_date, Time.now());
            values.Add(Tasks.done, Tools.bool_to_1_or_0(false));

            string row = SQLite.add(Tables.tasks, values);

            if (string.IsNullOrWhiteSpace(row))
            {
                string message = $"не удалось добавить новую строку для новой задачи в таблицу {Tables.tasks}";
            }

            Dictionary<string, string> hierarchy = new Dictionary<string, string>();
            hierarchy.Add(Hierachy.parent, id);
            hierarchy.Add(Hierachy.child, row);

            string row_project = SQLite.add(Tables.hierarchy, hierarchy);

            if (string.IsNullOrWhiteSpace(row_project))
            {
                string message = $"не удалось добавить новую строку для новой задачи в таблицу {Tables.hierarchy}";
            }
        }

        public void add_task_for_today()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add(Planner.date, Time.now_date());
            value.Add(Planner.task, id);

            string row = SQLite.add(Tables.planner, value);

            if (string.IsNullOrWhiteSpace(row))
            {
                string message = $"не удалось добавить новую строку для новой задачи на сегодня в таблицу {Tables.planner}";
            }
        }

        public void delete_task()
        {
            if (subtasks != null)
                for (int i = 0; i < subtasks.Count; i++)
                    subtasks[i].delete_task();

            SQLite.delete(Tables.tasks, Tasks.Id, id);

            SQLite.delete(Tables.hierarchy, Hierachy.child, id);
            // удачная находнка - инкапсуляция. удаляем не по ссылке на родителя или какие-то другие объекты,
            // а все строки, в которые входит текущая задача. и убирается необходимость в поле id родителя.
        }

        public void delete_task_from_today()
        {
            SQLite.delete_task_from_today(id);
        }


        public void form()
        {
            List<string> subtasks_id = SQLite.get_subtasks_id(id);

            subtasks = SQLite.get_subtasks(id);

            if (subtasks != null)
                for (int i = 0; i < subtasks.Count; i++)
                    subtasks[i].form();
        }

        public void set_description()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add(Tasks.description, description);

            Dictionary<string, string> where = this_task();

            SQLite.update(Tables.tasks, value, where);
        }

        public void set_done()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add(Tasks.done, Tools.bool_to_1_or_0(done));
            value.Add(Tasks.completion_date, Time.now());

            Dictionary<string, string> where = this_task();

            SQLite.update(Tables.tasks, value, where);
        }

        public void set_name()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add(Tasks.name, name);

            Dictionary<string, string> where = this_task();

            SQLite.update(Tables.tasks, value, where);
        }


        // today
        public void check_for_today()
        {
            if (Manager.tasks_for_today.Contains(id))
                added_for_today = true;

            if (subtasks != null)
                for (int i = 0; i < subtasks.Count; i++)
                    subtasks[i].check_for_today();
        }

        public bool check_for_today_back()
        {
            bool today = false;

            if (subtasks != null)
                for (int i = 0; i < subtasks.Count; i++)
                    if (added_for_today || subtasks[i].check_for_today_back())
                    {
                        today = true;
                        added_for_today = true;
                    }

            return today;
        }

        public void create_today_only_structure()
        {
            if (subtasks != null)
                for (int i = subtasks.Count; i >= 0; i--)
                    if (!subtasks[i].added_for_today) subtasks.RemoveAt(i);
                    else subtasks[i].create_today_only_structure();                    
        }

    }
}
