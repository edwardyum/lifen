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
    internal class SubTask
    {
        // оранизация
        private int id = 0;                 // id в таблице со всеми задачами в базе данных

        private string task_id = "";        // индекс задачи, в которой находится эта подзадача
        private int row = 0;                // номер строки в таблице проекта в программе

        // суть
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                Manager.update_task_in_db("name", value, id);
            }
        }

        private bool done;

        public bool Done
        {
            get { return done; }
            set
            {
                done = value;
                Manager.update_task_in_db("done", value.ToString(), id);
                Manager.update_task_in_db(Tasks.completion_date, Global.date_for_writing_in_db(), id);
            }
        }


        private readonly CommandBase delete_subtask_command_;
        public ICommand delete_subtask_command => delete_subtask_command_;

        private readonly CommandBase add_subtask_to_today_command_;
        public ICommand add_subtask_to_today_command => add_subtask_to_today_command_;


        public SubTask(string Task_id, int Id)
        {
            id = Id;
            task_id = Task_id;

            delete_subtask_command_ = new CommandBase(() => delete_subtask());
            add_subtask_to_today_command_ = new CommandBase(() => add_subtask_to_today());
        }

        private void add_subtask_to_today()
        {
            // добавляем задачу на сегодня
            add_task_today(Convert.ToInt32(task_id));

            // добавляем подзадачу на сегодня
            add_task_today(id);
        }


        private void add_task_today(int idl)
        {
            if (idl == 0)
            {
                Log.log("при попытке добавить задачу в список на сегодня обнаружено, что id задачи = 0. задача не добавлена на сегодня.");
            }

            if (check_if_task_already_planned_for_today())
                return;

            Dictionary<string, string> values = new Dictionary<string, string>();
            string date = Global.date_for_writing_in_db();

            values.Add("date", date);
            values.Add("task_id", idl.ToString());

            DBsql.insert_row_in_table_sql(Tables.planner, values);
        }

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



        private void delete_subtask()
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
    }
}
