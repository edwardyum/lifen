using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _2_assemble
{

    public delegate void refresh_data();


    internal static class Manager
    {
        static string connction_string = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\e.yumagulov\Desktop\системы\program\program\1_db\db\db.mdf;Integrated Security=True";
        //static string connction_string = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=F:\проекты\program\program\1_db\db\db.mdf;Integrated Security=True";

        public static List<Project> projects { get; set; }

        public static Today today { get; set; } = new Today();

        
        public static event refresh_data refresh_event;



        public static DataSet data;
        //private static Dictionary<string, SqlDataAdapter> adapters = new Dictionary<string, SqlDataAdapter>();


        public static readonly string table_id = "hierarchy";
        public static readonly string table_tasks = "tasks";

        private static DataTable projects_db;

        private static void initialize()
        {
            data = null;
            data = new DataSet();

            projects_db = new DataTable();
            projects = new List<Project>();
        }

        public static void connect ()
        {
            DBsql.InitializeComponent(connction_string);
            DBsql.open();
        }

        public static void disconnect()
        {
            DBsql.close();
        }

        public static void execute()
        {
            initialize();
            if (DBsql.connection == null || DBsql.connection.State != ConnectionState.Open)
            {
                connect();
            }

            get_my_projects();
            get_tasks();
            form_projects();
            
            today.execute();

            refresh_event.Invoke();

            //stored_procedure_without_parameters();
            //stored_procedure_with_parameters(1);
            //stored_procedure_with_SCOPE_IDENTITY(108);
            //stored_delete_task(155);
            //stored_add_task_to_today(142, Global.date_for_writing_in_db());
            //stored_delete_task_from_today(142);
            //stored_set_done(1171, false);
            //stored_set_name(1171, "новое название");
            //stored_set_description(1171, "новое описание");
        }

        private static void get_my_projects()
        {            
            // получение имён и id проектов
            // 1 - root
            string sql_names = $"SELECT {table_id}.child AS 'Id', {table_tasks}.name FROM {table_id}, {table_tasks} WHERE {table_id}.parent = 1 AND {table_id}.child = {table_tasks}.Id";

            SqlDataAdapter adapter = new SqlDataAdapter(sql_names, DBsql.connection);

            adapter.Fill(data, "projects");

            projects_db = data.Tables["projects"];
        }

        private static void get_tasks()
        {
            // получение таблиц проектов
            string sql_content = "";

            for (int i = 0; i < projects_db.Rows.Count; i++)
            {
                sql_content = $"SELECT * FROM {table_tasks}, {table_id} WHERE parent = {projects_db.Rows[i][0]} AND {table_id}.child = {table_tasks}.Id";

                SqlDataAdapter adapter = new SqlDataAdapter(sql_content, DBsql.connection);
                adapter.Fill(data, projects_db.Rows[i][0].ToString());
            }
        }

        private static void form_projects()
        {
            for (int i = 0; i < projects_db.Rows.Count; i++)
            {
                Project project = new Project() { 
                    Id = Convert.ToInt32(projects_db.Rows[i]["Id"]),
                    Name = projects_db.Rows[i]["name"].ToString()};

                projects.Add(project);

                // ВВЕСТИ ПРОВЕРКУ НА СУЩЕСТВОВАНИЕ ТАБЛИЦ ПРОЕКТОВ
                
                DataTable pr = data.Tables[project.Id.ToString()];
                for (int j = 0; j < pr.Rows.Count; j++)
                {
                    MyTask task = new MyTask(project.Id.ToString(), Convert.ToInt32(pr.Rows[j]["Id"]), j);
                    task.Name = pr.Rows[j]["name"].ToString();
                    
                    string done = pr.Rows[j]["done"].ToString();
                    if (done == "") done = "false";
                    task.Done = Convert.ToBoolean(done);

                    task.form_task();

                    project.Tasks.Add(task);
                }
            }
        }


        public static void update_task_in_db(string field, string value, int id)
        {
            string table = "tasks";
            Dictionary<string, string> values = new Dictionary<string, string>() { { field, value } };
            Dictionary<string, string> where = new Dictionary<string, string>() { { "Id", id.ToString() } };

            DBsql.update_row_in_table_sql(table, values, where);
        }



        public static void test()
        {
            DBsql.InitializeComponent(connction_string);
            DBsql.open();


            string table_id = "hierarchy";
            string table_tasks = "tasks";
            // получение имён и id проектов
            // 1 - root
            //string sql_names = $"SELECT child FROM {table} WHERE parent = 1";
            string sql_names = $"SELECT {table_id}.child AS 'Id', {table_tasks}.name FROM {table_id}, {table_tasks} WHERE {table_id}.parent = 1 AND {table_id}.child = {table_tasks}.Id";
            //string sql = $"SELECT * FROM [{table}]";

            SqlDataAdapter adapter = new SqlDataAdapter(sql_names, DBsql.connection);
            

            adapter.Fill(data, "projects");

            DataTable projects = data.Tables["projects"];

            // получение таблиц проектов
            string sql_content = "";

            for (int i = 0; i < projects.Rows.Count; i++)
            {
                //sql_content = $"SELECT child FROM {table_id} where parent = {projects.Rows[i][0]}";
                sql_content = $"SELECT * FROM {table_tasks}, {table_id} WHERE parent = {projects.Rows[i][0]} AND {table_id}.child = {table_tasks}.Id";
                SqlDataAdapter adapter_content = new SqlDataAdapter(sql_content, DBsql.connection);
                adapter_content.Fill(data, projects.Rows[i][1].ToString());
            }


            int max = 0;

            for (int i = 0; i < projects.Rows.Count; i++)
            {
                int current = data.Tables[projects.Rows[i][1].ToString()].Rows.Count;
                if (current > max)
                    max = current;
            }


            DataTable show = new DataTable();

            for (int i = 0; i < max; i++)
                show.Rows.Add();

            for (int i = 0; i < projects.Rows.Count; i++)
            {
                string project_name = projects.Rows[i][1].ToString();

                show.Columns.Add(project_name);

                for (int j = 0; j < data.Tables[project_name].Rows.Count; j++)
                {
                    show.Rows[j][project_name] = data.Tables[project_name].Rows[j]["name"].ToString();
                }
            }

            


            //
            DBsql.close();
        }

        // ДОБАВИТЬ В БИБЛИОТЕКУ БАЗЫ ДАННЫХ
        private static void stored_procedure_without_parameters()
        {
            string sql = StoredProcedures.get_projects;

            SqlCommand command = new SqlCommand(sql,DBsql.connection);
            command.CommandType = CommandType.StoredProcedure;

            //var result = command.ExecuteScalar();

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(2);

                    // ЛУЧШЕ ПОЛЬЗОВАТЬ ТАКИМ ПОДХОДОМ. ЕСЛИ МЕНЯЮТСЯ ПОЛЯ В БАЗЕ ДАННЫХ, ТО ОН СОХРАНЯЕТ РАБОТОСПОСОБНОСТЬ.
                    int id_1 = (int)reader[Tasks.Id];
                    string name_1 = (string)reader[Tasks.name];

                    MessageBox.Show($"{id}\n" +
                        $"{name}\n" +
                        $"{id_1}\n" +
                        $"{name_1}\n");
                }
            }
            
            
            // если процедура ничего не возвращает
            //var result = command.ExecuteNonQuery();

            // если вернуть число
            //var result = command.ExecuteScalar();
        }

        // ДОБАВИТЬ В БИБЛИОТЕКУ БАЗЫ ДАННЫХ
        private static void stored_procedure_with_parameters(int idt)
        {
            string sql = StoredProcedures.get_tasks;

            SqlCommand command = new SqlCommand(sql, DBsql.connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter id_task = new SqlParameter
            {
                ParameterName = "@id_task",
                Value = idt
            };

            command.Parameters.Add(id_task);

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(2);

                    // ЛУЧШЕ ПОЛЬЗОВАТЬ ТАКИМ ПОДХОДОМ. ЕСЛИ МЕНЯЮТСЯ ПОЛЯ В БАЗЕ ДАННЫХ, ТО ОН СОХРАНЯЕТ РАБОТОСПОСОБНОСТЬ.
                    int id_1 = (int)reader[Tasks.Id];
                    string name_1 = (string)reader[Tasks.name];

                    MessageBox.Show($"{id}\n" +
                        $"{name}\n" +
                        $"{id_1}\n" +
                        $"{name_1}\n");
                }
            }


            // если процедура ничего не возвращает
            //var result = command.ExecuteNonQuery();

            // если вернуть число
            //var result = command.ExecuteScalar();
        }

        // ДОБАВИТЬ В БИБЛИОТЕКУ БАЗЫ ДАННЫХ
        private static void stored_procedure_with_SCOPE_IDENTITY(int idu)
        {
            string sql = StoredProcedures.add_task;

            SqlCommand command = new SqlCommand(sql, DBsql.connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter data = new SqlParameter { ParameterName = "@data", Value = Global.datetime_for_writing_in_db() };
            SqlParameter id_uptask = new SqlParameter { ParameterName = "@id_uptask", Value = idu };

            command.Parameters.Add(data);
            command.Parameters.Add(id_uptask);

            int res = command.ExecuteNonQuery();

            //if (reader.HasRows)
            //{
            //    while (reader.Read())
            //    {
            //        int id = reader.GetInt32(0);
            //        string name = reader.GetString(2);

            //        // ЛУЧШЕ ПОЛЬЗОВАТЬ ТАКИМ ПОДХОДОМ. ЕСЛИ МЕНЯЮТСЯ ПОЛЯ В БАЗЕ ДАННЫХ, ТО ОН СОХРАНЯЕТ РАБОТОСПОСОБНОСТЬ.
            //        int id_1 = (int)reader[Tasks.Id];
            //        string name_1 = (string)reader[Tasks.name];

            //        MessageBox.Show($"{id}\n" +
            //            $"{name}\n" +
            //            $"{id_1}\n" +
            //            $"{name_1}\n");
            //    }
            //}


            // если процедура ничего не возвращает
            //var result = command.ExecuteNonQuery();

            // если вернуть число
            //var result = command.ExecuteScalar();
        }

        private static void stored_delete_task(int idt)
        {
            string sql = StoredProcedures.delete_task;

            SqlCommand command = new SqlCommand(sql, DBsql.connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter id_task = new SqlParameter
            {
                ParameterName = "@Id",
                Value = idt
            };

            command.Parameters.Add(id_task);

            int result = command.ExecuteNonQuery();
        }

        private static void stored_add_task_to_today(int idt, string date_today)
        {
            string sql = StoredProcedures.add_task_to_today;

            SqlCommand command = new SqlCommand(sql, DBsql.connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter task_id = new SqlParameter
            {
                ParameterName = "@task_id",
                Value = idt
            };

            SqlParameter date = new SqlParameter
            {
                ParameterName = "@date",
                Value = date_today
            };

            command.Parameters.Add(task_id);
            command.Parameters.Add(date);

            int result = command.ExecuteNonQuery();
        }

        private static void stored_delete_task_from_today(int idt)
        {
            string sql = StoredProcedures.delete_task_from_today;

            SqlCommand command = new SqlCommand(sql, DBsql.connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter task_id = new SqlParameter
            {
                ParameterName = "@task_id",
                Value = idt
            };

            command.Parameters.Add(task_id);

            int result = command.ExecuteNonQuery();
        }

        private static void stored_set_done(int idt, bool have_done)
        {
            string sql = StoredProcedures.set_done;

            SqlCommand command = new SqlCommand(sql, DBsql.connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter task_id = new SqlParameter
            {
                ParameterName = "@Id",
                Value = idt
            };

            SqlParameter done = new SqlParameter
            {
                ParameterName = "@done",
                Value = have_done
            };

            SqlParameter completion_date = new SqlParameter
            {
                ParameterName = "@completion_date",
                Value = Global.datetime_for_writing_in_db()
            };

            command.Parameters.Add(task_id);
            command.Parameters.Add(done);
            command.Parameters.Add(completion_date);

            int result = command.ExecuteNonQuery();
        }

        private static void stored_set_name(int idt, string name_)
        {
            string sql = StoredProcedures.set_name;

            SqlCommand command = new SqlCommand(sql, DBsql.connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter task_id = new SqlParameter
            {
                ParameterName = "@Id",
                Value = idt
            };

            SqlParameter name = new SqlParameter
            {
                ParameterName = "@name",
                Value = name_
            };

            command.Parameters.Add(task_id);
            command.Parameters.Add(name);

            int result = command.ExecuteNonQuery();
        }

        private static void stored_set_description(int idt, string description_)
        {
            string sql = StoredProcedures.set_description;

            SqlCommand command = new SqlCommand(sql, DBsql.connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter task_id = new SqlParameter
            {
                ParameterName = "@Id",
                Value = idt
            };

            SqlParameter description = new SqlParameter
            {
                ParameterName = "@description",
                Value = description_
            };

            command.Parameters.Add(task_id);
            command.Parameters.Add(description);

            int result = command.ExecuteNonQuery();
        }


    }
}
