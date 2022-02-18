using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_assemble
{


    // В ЭТОМ КЛАССЕ МНОГО МЕТОДОВ И ПОДХОДОВ ДУБЛИРУЕТ РАБОТУ КЛАССА MANAGER


    internal class Today
    {
        public List<Project> projects { get; set; } = new List<Project>();

        public void execute()
        {
            projects.Clear();

            DataTable tasks_today = get_today_tasks();
            DataTable tasks_today2 = get_tasks_today();
            DataTable projects_table = get_projects_();

            form_projects_today(projects_table);

            DataSet task_in_projects = get_tasks_id_of_project(projects);
            form_tasks(task_in_projects, tasks_today2);
        }

        private DataTable get_today_tasks()
        {
            string date = Global.date_for_reading_from_db();

            Dictionary<string, string> where = new Dictionary<string, string>();
            where.Add(Planner.date, date);

            DataTable tasks_today = DBsql.get_rows_in_table(Tables.planner, where);

            return tasks_today;
        }

        //ЭТОТ МЕТОД ДУБЛИРУЕТ МЕТОД ФОРМИРОВАНИЯ ПРОЕКТОВ В MANAGER
        private DataTable get_projects_()
        {
            // получение имён и id проектов
            // 1 - root
            string sql_names = $"SELECT {Tables.hierarchy}.child AS 'Id', {Tables.tasks}.name FROM {Tables.hierarchy}, {Tables.tasks} WHERE {Tables.hierarchy}.parent = 1 AND {Tables.hierarchy}.child = {Tables.tasks}.Id";

            SqlDataAdapter adapter = new SqlDataAdapter(sql_names, DBsql.connection);

            DataSet data = new DataSet();
            adapter.Fill(data, "projects");

            return data.Tables["projects"];
        }

        private DataSet get_tasks_id_of_project(List<Project> projects_)
        {
            DataSet data = new DataSet();

            foreach (Project project in projects_)
            {
                string sql = $"SELECT {Hierachy.child} FROM {Tables.hierarchy} WHERE {Hierachy.parent} = {project.Id}";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, DBsql.connection);
                adapter.Fill(data, project.Id.ToString());
            }

            return data;
        }

        private DataTable get_tasks_today()
        {
            string today = Global.date_for_reading_from_db();

            string sql = $"SELECT * FROM {Tables.tasks}, {Tables.planner} WHERE {Tables.tasks}.{Tasks.Id} = {Tables.planner}.{Planner.task_id} " +
                $"AND {Tables.planner}.{Planner.date} = {today}";

            DataSet data = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(sql, DBsql.connection);
            adapter.Fill(data, "today");

            return data.Tables[0];
        }

        //ЭТОТ МЕТОД ДУБЛИРУЕТ МЕТОД ФОРМИРОВАНИЯ ПРОЕКТОВ В MANAGER
        private void form_projects_today(DataTable pr)
        {
            for(int i = 0; i < pr.Rows.Count; i++)
            {
                Project project = new Project()
                {
                    Id = Convert.ToInt32(pr.Rows[i]["Id"]),
                    Name = pr.Rows[i]["name"].ToString()
                };

                projects.Add(project);
            }
        }

        private void form_tasks_in_project(Project project, DataTable pr)
        {
            for (int j = 0; j < pr.Rows.Count; j++)
            {
                MyTask task = new MyTask(project.Id.ToString(), Convert.ToInt32(pr.Rows[j]["Id"]), j);
                task.Name = pr.Rows[j]["name"].ToString();

                string done = pr.Rows[j]["done"].ToString();
                if (done == "") done = "false";
                task.Done = Convert.ToBoolean(done);

                project.Tasks.Add(task);
            }
        }

        private void form_tasks(DataSet tasks, DataTable tasks_today)
        {
            foreach (var project in projects)
            {
                DataTable pr = tasks.Tables[project.Id.ToString()];

                foreach (DataRow row in tasks_today.Rows)
                {
                    int col = 0;

                    if ( DataTable_Column_contains_value(row[0].ToString(), pr, 0, out col))
                    {
                        // В КОНСТРУКТОРЕ ИНДЕКС СТРОКИ ВЫСТАВЛЕН КАК 0 - ИЗ ЗА ПЕРЕБОРА СТРОК
                        // ЭТА СТРОКА ИСПОЛЬЗУЕТСЯ ТОЛЬКО ДЛЯ ВНУТРЕННЕЙ МОДЕЛИ. ХОТЯ НА ПРАКТИКЕ РАБОТАЮ ТОЛЬКО С ВНЕШНЕЙ

                        MyTask task = new MyTask(project.Id.ToString(), Convert.ToInt32(row["Id"]), 0);
                        task.Name = row["name"].ToString();

                        string done = row["done"].ToString();
                        if (done == "") done = "false";
                        task.Done = Convert.ToBoolean(done);

                        task.form_task();

                        project.Tasks.Add(task);
                    }                    
                }
            }            
        }


        


        // ДОБАВИТЬ В БИБЛИОТЕКУ

        private bool DataTable_contains_value(DataTable dt, string val)                     // содержится ли в таблице значение
        {
            // не учтено что может быть много найденных значений

            for (int i = 0; i < dt.Rows.Count; i++)
                for (int j = 0; j < dt.Columns.Count; j++)
                    if( dt.Rows[i][j].ToString() == val ) return true;
                
            return false;
        }

        // ДОБАВИТЬ В БИБЛИОТЕКУ

        private bool DataTable_Column_contains_value(string val, DataTable dt, int column, out int col)     // содержится ли в столбце таблицы значение
        {
            // не учтено что может быть много найденных значений

            for (int i = 0; i < dt.Rows.Count; i++)
                if (dt.Rows[i][column].ToString() == val) { col = i; return true; }

            col = 0;
            return false;
        }



    }
}
