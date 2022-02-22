using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{
    internal static class DBInteraction
    {
        public static ObservableCollection<Objective> get_projects()
        {
            ObservableCollection<Objective> projects = new ObservableCollection<Objective>();

            string sql = StoredProcedures.get_projects;

            SqlCommand command = new SqlCommand(sql, DBS.connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string id = Convert.ToString(reader[Tasks.Id]);

                    Objective project = new Objective(id, "1");
                    project.obtaining_data_from_db = true;

                    project.Name = Convert.ToString(reader[Tasks.name]);
                    project.Description = Convert.ToString(reader[Tasks.description]);

                    object Don = reader[Tasks.done]; if( Don.GetType() != typeof( DBNull)) project.Done = Convert.ToBoolean(Don);

                    project.DataCompletion = Convert.ToString(reader[Tasks.completion_date]);
                    project.DataCreation = Convert.ToString(reader[Tasks.creation_date]);

                    project.obtaining_data_from_db = false;
                    projects.Add(project);
                }
            }

            reader.Close();
            return projects;
        }

        public static ObservableCollection<Objective> get_tasks(string idt)
        {
            ObservableCollection<Objective> tasks = new ObservableCollection<Objective>();

            string sql = StoredProcedures.get_tasks;

            SqlCommand command = new SqlCommand(sql, DBS.connection);
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
                    string id = Convert.ToString(reader[Tasks.Id]);

                    Objective objective = new Objective(id, idt);
                    objective.obtaining_data_from_db = true;

                    objective.Name = Convert.ToString(reader[Tasks.name]);
                    objective.Description = Convert.ToString(reader[Tasks.description]);

                    object Don = reader[Tasks.done]; if (Don.GetType() != typeof(DBNull)) objective.Done = Convert.ToBoolean(Don);

                    objective.DataCompletion = Convert.ToString(reader[Tasks.completion_date]);
                    objective.DataCreation = Convert.ToString(reader[Tasks.creation_date]);

                    objective.obtaining_data_from_db = false;
                    tasks.Add(objective);
                }
            }
            
            reader.Close();
            return tasks;
        }

        public static void add_task(int idu)
        {
            string sql = StoredProcedures.add_task;

            SqlCommand command = new SqlCommand(sql, DBS.connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter data = new SqlParameter { ParameterName = "@data", Value = Global.datetime_for_writing_in_db() };
            SqlParameter id_uptask = new SqlParameter { ParameterName = "@id_uptask", Value = idu };

            command.Parameters.Add(data);
            command.Parameters.Add(id_uptask);

            int res = command.ExecuteNonQuery();
        }

        public static void delete_task(int idt)
        {
            string sql = StoredProcedures.delete_task;

            SqlCommand command = new SqlCommand(sql, DBS.connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter id_task = new SqlParameter
            {
                ParameterName = "@Id",
                Value = idt
            };

            command.Parameters.Add(id_task);

            int result = command.ExecuteNonQuery();
        }

        public static void add_task_to_today(int idt, string date_today)
        {
            string sql = StoredProcedures.add_task_to_today;

            SqlCommand command = new SqlCommand(sql, DBS.connection);
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

        public static void delete_task_from_today(int idt)
        {
            string sql = StoredProcedures.delete_task_from_today;

            SqlCommand command = new SqlCommand(sql, DBS.connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter task_id = new SqlParameter
            {
                ParameterName = "@task_id",
                Value = idt
            };

            command.Parameters.Add(task_id);

            int result = command.ExecuteNonQuery();
        }

        public static void set_done(int idt, bool have_done)
        {
            string sql = StoredProcedures.set_done;

            SqlCommand command = new SqlCommand(sql, DBS.connection);
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

        public static void set_name(int idt, string name_)
        {
            string sql = StoredProcedures.set_name;

            SqlCommand command = new SqlCommand(sql, DBS.connection);
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

        public static void set_description(int idt, string description_)
        {
            string sql = StoredProcedures.set_description;

            SqlCommand command = new SqlCommand(sql, DBS.connection);
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
