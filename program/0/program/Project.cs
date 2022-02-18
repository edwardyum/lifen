using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace program
{
    class Project
    {
        public int id;
        public bool was_changed = false;

        public string name = "";

        public ProjectStatus status = ProjectStatus.In_work;
        
        List<Tasks> tasks = new List<Tasks>();


        public void new_task()
        {

        }

        public void change_order_of_task()
        {

        }

        public void relocate_Task_to_another_Project()
        {

        }

        public void change_task()
        {

        }

        public void complete_task()
        {

        }

        public void cancel_task()
        {

        }

        public void resume_task()
        {

        }

        public void delete_task()
        {

        }


    }
}
