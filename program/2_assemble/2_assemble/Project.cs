using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace _2_assemble
{
    internal class Project
    {
        public int Id { get; set; }

        private string name;
        public string Name {
            get { return name; }
            set
            {
                name = value;
                //Manager.data.Tables["projects"].Rows[row]["name"] = value;
                Manager.update_task_in_db("name", value, Id);
            }
        }
        public string Description { get; set; }

        public List<MyTask> Tasks { get; set; } = new List<MyTask>();


        private readonly CommandBase new_task_command_;
        public ICommand new_task_command => new_task_command_;

        private readonly CommandBase delete_project_command_;
        public ICommand delete_project_command => delete_project_command_;


        public Project()
        {
            new_task_command_ = new CommandBase(new_task);
            delete_project_command_ = new CommandBase(() => delete_project(Id.ToString()));
        }

        private void new_task()
        {
            // добавляем новую строку в таблицу task
            Dictionary<string,string> values=new Dictionary<string,string>();
            values.Add("name", "новая задача");

            DBsql.insert_row_in_table_sql(Manager.table_tasks, values);

            // добавляем новую строку в таблицу иерархии
            DataTable last_row = DBsql.get_last_row_in_table(Manager.table_tasks);

            string id_of_new_row_in_task = last_row.Rows[0]["Id"].ToString();

            Dictionary<string, string> values_project = new Dictionary<string, string>();
            values_project.Add("parent", Id.ToString());
            values_project.Add("child", id_of_new_row_in_task);

            DBsql.insert_row_in_table_sql(Manager.table_id, values_project);

            Manager.execute();
        }

        private void delete_project(string id)
        {
            // удаляем все входящие в проект задачи
            if (Tasks != null)
            {
                for (int i = 0; i < Tasks.Count; i++)
                {
                    Tasks[i].delete_task();
                }
            }

            // получаем строку для проверки вхождения строки
            Dictionary<string, string> where = new Dictionary<string, string>();
            where.Add("child", id);

            DataTable row_in_hierarchy = DBsql.get_rows_in_table(Manager.table_id, where);
            if (row_in_hierarchy == null || row_in_hierarchy.Rows.Count == 0)
            {
                Log.log("такой строки не обраружено в таблице иерархий");
            }

            // удаляем задачу из списка задач
            DBsql.delete_row_in_table_sql(Manager.table_tasks, "Id", id);

            // удаляем задачу мз таблицы иерархии
            DBsql.delete_row_in_table_sql(Manager.table_tasks, "child", id);

            Manager.execute();
        }

        

    }
}
