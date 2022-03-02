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
        private string id_uptask = string.Empty;

        //
        private string name = string.Empty;
        private bool   done = false;
        private string description = string.Empty;
        private string data_creation = string.Empty;
        private string data_completion = string.Empty;

        public bool added_to_today = false;

        //
        public bool obtaining_data_from_db = false;
        public string Name { get { return name; } set { name = value; if (!obtaining_data_from_db) { }} }
        public bool Done { get { return done; } set { done = value; if (!obtaining_data_from_db) { } } }
        public string Description { get { return description; } set { description = value; if (!obtaining_data_from_db) { } } }
        public string DataCreation { get { return data_creation; } set { data_creation = value; if (!obtaining_data_from_db) { } } }
        public string DataCompletion { get { return data_completion; } set { data_completion = value; if (!obtaining_data_from_db) { } } }


        public ObservableCollection<Objective> subtasks { get; set; } = new ObservableCollection<Objective>();


        public Objective(string id_, string id_uptask_)
        {
            id = id_;
            id_uptask = id_uptask_;
        }

        public void form_subtasks()
        {
            subtasks = DBInteraction.get_tasks(id);

            for (int i = 0; i < subtasks.Count; i++)
                subtasks[i].form_subtasks();
        }

        private void create_subtask()
        {

        }

        private void delete_task()
        {

        }

        private void add_task_to_today()
        {

        }


    }
}
