using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_assemble
{
    internal class Objective
    {
        //
        private string id               = string.Empty;
        private string id_uptask        = string.Empty;

        //
        private string name             = string.Empty;
        private bool   done             = false;
        private string description      = string.Empty;
        private string data_creation    = string.Empty;
        private string data_completion  = string.Empty;

        public bool    added_to_today   = false;

        public List<Objective> subtasks { get; set; } = new List<Objective>();


        public Objective(string id_, string id_uptask_)
        {
            id = id_;
            id_uptask = id_uptask_;
        }
        

        private void get_subrasks_()
        {

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
