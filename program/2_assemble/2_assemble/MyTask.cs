using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace _2_assemble
{
    internal class MyTask
    {
        // оранизация
        private int id = 0;                 // id в таблице со всеми задачами в базе данных

        private string project_id = "";     // имя таблицы, в которой находится задача
        private int row = 0;                // номер строки в таблице проекта в программе

        // суть

        public List<SubTask> subtasks { get; set; } = new List<SubTask>();

        private string name;
        public string Name { 
            get { return name; } 
            set { 
                name = value;
                //Manager.data.Tables[project_id].Rows[row]["name"] = value;
                Manager.update_task_in_db("name", value, id);
            } }

        private bool done;

        public bool Done {
            get { return done; }
            set
            {
                done = value;
                //Manager.data.Tables[project_id].Rows[row]["done"] = value;
                Manager.update_task_in_db("done", value.ToString(), id);
                Manager.update_task_in_db(Tasks.completion_date, Global.date_for_writing_in_db(), id);
            }
        }


        private readonly CommandBase delete_task_command_;
        public ICommand delete_task_command => delete_task_command_;

        private readonly CommandBase add_task_to_today_command_;
        public ICommand add_task_to_today_command => add_task_to_today_command_;

        private readonly CommandBase add_subtask_command_;
        public ICommand add_subtask_command => add_subtask_command_;


        public MyTask(string Project_id, int Id, int Row)
        {
            id = Id;
            project_id = Project_id;
            row = Row;

            delete_task_command_ = new CommandBase(() => delete_task());
            add_task_to_today_command_ = new CommandBase(() => add_task_to_today());
            add_subtask_command_ = new CommandBase(() => new_subtask());
            //form_task();
        }

        public void form_task()
        {
            // получить список подзадач
            DataTable subt = get_subtasks_();

            // сформировать список подзадач

            for (int i = 0; i < subt.Rows.Count; i++)
            {
                SubTask subTask = new SubTask(id.ToString(), Convert.ToInt32(subt.Rows[i][0]));
                subTask.Name = subt.Rows[i]["name"].ToString();

                string done = subt.Rows[i]["done"].ToString();
                if (done == "") done = "false";
                subTask.Done = Convert.ToBoolean(done);

                subtasks.Add(subTask);
            }
        }

        private DataTable get_subtasks_()
        {
            // получение таблиц проектов
            string sql = $"SELECT * FROM {Tables.tasks}, {Tables.hierarchy} WHERE {Hierachy.parent} = {id} AND " +
                                                                                 $"{Hierachy.child} = {Tables.tasks}.{Tasks.Id}";

            DataSet data = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(sql, DBsql.connection);
            adapter.Fill(data, id.ToString());

            return data.Tables[0];
        }



        public void delete_task()
        {
            string ids = id.ToString();

            // получаем строку для проверки вхождения строки
            Dictionary<string, string> where = new Dictionary<string, string>();
            where.Add("child", ids);

            DataTable row_in_hierarchy = DBsql.get_rows_in_table(Manager.table_id, where);
            if (row_in_hierarchy == null || row_in_hierarchy.Rows.Count == 0)
            {
                Log.log("такой строки не обраружено в таблице иерархий");
            }

            // удаляем задачу из списка задач
            DBsql.delete_row_in_table_sql(Manager.table_tasks, "Id", ids);

            // удаляем задачу мз таблицы иерархии
            DBsql.delete_row_in_table_sql(Manager.table_tasks, "child", ids);

            Manager.execute();
        }


        // ПОВТОРЯЕТ ДОБАВЛЕНИЕ ЗАДАЧИ В ПРОЕКТ
        private void new_subtask()
        {
            // новая задача в списке задач
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("name", "новая задача");

            DBsql.insert_row_in_table_sql(Manager.table_tasks, values);

            // прикрипление подзадачи к задаче
            DataTable last_row = DBsql.get_last_row_in_table(Manager.table_tasks);

            string id_of_new_row_in_task = last_row.Rows[0]["Id"].ToString();

            Dictionary<string, string> values_project = new Dictionary<string, string>();
            values_project.Add("parent", id.ToString());
            values_project.Add("child", id_of_new_row_in_task);

            DBsql.insert_row_in_table_sql(Manager.table_id, values_project);

            Manager.execute();
        }

        public void add_task_to_today()
        {
            if (id == 0)
            {
                Log.log("при попытке добавить задачу в список на сегодня обнаружено, что id задачи = 0. задача не добавлена на сегодня.");
            }

            if (check_if_task_already_planned_for_today())            
                return;

            Dictionary<string , string> values = new Dictionary<string, string>();
            string date = Global.date_for_writing_in_db();

            values.Add("date", date);
            values.Add("task_id", id.ToString());

            DBsql.insert_row_in_table_sql(Tables.planner, values);
        }


        // МЕТОД ПЕРЕВЕСТИ В ХРАНИМУЮ ФУНКЦИЮ БАЗЫ ДАННЫХ
        private bool check_if_task_already_planned_for_today()
        {
            bool exists = false;

            if (id == 0)
            {
                Log.log("при попытке добавить задачу в список на сегодня обнаружено, что id задачи = 0. задача не добавлена на сегодня.");
            }
            string date = Global.date_for_reading_from_db();

            string sql = $"SELECT * FROM {Tables.planner} WHERE {Planner.date}={date} AND {Planner.task_id}={id}";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, DBsql.connection);
            DataSet data = new DataSet();

            try
            {
                Log.log($"выполняется sql команда: {sql}");
                adapter.Fill(data);
            }
            catch (Exception ex)
            {
                Log.log($"ошибка при выполнении кодманды получения всех влючений задачи в планнере на сегодня: {ex.Message}");
            }

            if (data.Tables[0].Rows.Count > 0)
                exists = true;
            else
                exists = false;

            return exists;
        }

    }
}
