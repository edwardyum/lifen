using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace lifen
{

    internal static class SQLite
    {
        private static string db_name = "db.db";
        private static string path_to_folder;
        private static string path;                 // полный путь к базе данных


        public static void template(string table, Dictionary<string, string> values)   // шаблон
        {
            string mes = $"при попытке обновить данные в базе данных";

            if (check_access(mes))
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={path}"))
                {
                    db.Open();

                    SqliteCommand command = new SqliteCommand();

                    command.Connection = db;

                    string correspondence = field_value_string_for_update_row(values);
                    string sql = $"UPDATE {table} SET {correspondence} WHERE ... '";

                    command.CommandText = sql;

                    try
                    {
                        command.ExecuteReader();
                    }
                    catch (Exception ex)
                    {
                        string message = $"{mes} база данных вернула следующую ошибку: {ex.Message}";
                    }

                    db.Close();
                }
            }
        }

        private static bool check_access(string mes = null)
        {
            bool access = true;

            if (path != null)
            {
                if (File.Exists(path))
                {
                    string message = $"база данных доступна";
                }
                else
                {
                    access = false;
                    string message = $"{mes} обнаружено, что программа не может найти файл базы данных по указанному пути: {path}";
                }
            }
            else
            {
                access = false;
                string message = $"{mes} обнаружено, что строка указывающая путь к базе данных = null";
            }

            return access;
        }


        public static bool check()
        {
            return true;
        }



        public static void initialize()
        {
            path_to_folder = Tools.get_local_folder();
            path = Path.Combine(path_to_folder, db_name);
        }

        public async static void create_db()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(db_name, CreationCollisionOption.OpenIfExists);
        }

        // не проработано
        public static void create_table()
        {
            if (path == null)
            {

            }
            else
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={path}"))
                {
                    db.Open();

                    string sql = "CREATE TABLE IF NOT EXISTS MyTable " +
                                 "(Primary_Key INTEGER PRIMARY KEY, Text_Entry NVARCHAR(2048) NULL)";

                    SqliteCommand command = new SqliteCommand(sql, db);

                    command.ExecuteReader();
                }
            }
        }

        public static string add(string table, Dictionary<string, string> values)   // добавление данных в таблицу
        {
            string id_row = string.Empty;

            string mes = "при попытке добавить данные в базу данных";

            if (check_access(mes))
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={path}"))
                {
                    db.Open();

                    SqliteCommand command = new SqliteCommand();

                    command.Connection = db;

                    Tuple<string, string> strings = fields_values_strings_for_row(values);

                    string sql = $"INSERT INTO {table} ({strings.Item1}) VALUES ({strings.Item2}); " +
                                 $"SELECT last_insert_rowid();";

                    command.CommandText = sql;

                    try
                    {
                        SqliteDataReader reader = command.ExecuteReader();
                        reader.Read();
                        id_row = reader.GetString(0);
                    }
                    catch (Exception ex)
                    {
                        string message = $"{mes} база данных вернула следующую ошибку: {ex.Message}";
                    }

                    db.Close();
                }
            }

            return id_row;
        }


        private static Tuple<string, string> fields_values_strings_for_row(Dictionary<string, string> fields)
        {
            // в этот метод следует отправлять словарь с названиями полей и значениями для них, которые были проверены на существование и соответствие их типам
            // метод выдаёт строку полей в формате "name, surname, birthday"
            // метод выдаёт строку значений в формате "N'андрей', N'андреев', N'12/09/2021'"


            if (fields == null || fields.Count == 0)
            {
                string message = $"запрошена команда на формирование строк для работы со строками в таблице в базе данных, онако входящий параметр Dictionary пуст или ==null." +
                                "процедура формирования строк прервана. строки не сформированы.";
                //Log.log(message);
                throw new Exception(message);
            }


            string f = "", v = "";

            foreach (var item in fields)
            {
                f += $"{item.Key}, ";
                v += $"'{item.Value}', ";
                //v += $"N'{item.Value}', ";
            }

            f = f.Remove(f.Length - 1);
            f = f.Remove(f.Length - 1);
            v = v.Remove(v.Length - 1);
            v = v.Remove(v.Length - 1);


            Tuple<string, string> strings = new Tuple<string, string>(f, v);

            return strings;
        }

        public static List<string> get_subtasks_id(string id)   // получить данные из базы данных
        {
            List<string> subtasks_id = new List<string>();

            string mes = $"при попытке получить id подзадач из таблицы {Tables.hierarchy}";

            if (check_access(mes))
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={path}"))
                {
                    db.Open();

                    string sql = $"SELECT {Hierachy.child} FROM {Tables.hierarchy} WHERE {Hierachy.parent}={id}";

                    SqliteCommand command = new SqliteCommand(sql, db);

                    try
                    {
                        SqliteDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                            subtasks_id.Add(reader.GetString(0));
                    }
                    catch (Exception ex)
                    {
                        string message = $"{mes} база данных вернула следующую ошибку: {ex.Message}";
                    }

                    db.Close();
                }
            }

            return subtasks_id;
        }

        public static ObservableCollection<Objective> get_subtasks(string id)   // получить данные из базы данных
        {
            ObservableCollection<Objective> tasks = new ObservableCollection<Objective>();

            string mes = "при попытке получить данные из базы данных";

            if (check_access(mes))
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={path}"))
                {
                    db.Open();

                    string sql = $"SELECT * FROM {Tables.tasks}, {Tables.hierarchy} WHERE {Tables.hierarchy}.{Hierachy.parent} = {id} " +
                    $"AND {Tables.tasks}.{Tasks.Id}={Tables.hierarchy}.{Hierachy.child}";

                    SqliteCommand command = new SqliteCommand(sql, db);

                    try
                    {
                        SqliteDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            int c = 0;

                            Objective task = new Objective(reader.GetString(0));
                            task.obtaining_data_from_db = true;

                            c = reader.GetOrdinal(Tasks.creation_date); task.DataCreation = reader.GetString(c);
                            c = reader.GetOrdinal(Tasks.name); task.Name = reader.GetString(c);
                            c = reader.GetOrdinal(Tasks.description); if (!reader.IsDBNull(c)) task.Description = reader.GetString(c);
                            c = reader.GetOrdinal(Tasks.done); if (!reader.IsDBNull(c)) task.Done = reader.GetBoolean(c);
                            c = reader.GetOrdinal(Tasks.completion_date); if (!reader.IsDBNull(c)) task.DataCompletion = reader.GetString(c);

                            task.obtaining_data_from_db = false;
                            tasks.Add(task);
                        }
                    }
                    catch (Exception ex)
                    {
                        string message = $"{mes} база данных вернула следующую ошибку: {ex.Message}";
                    }

                    db.Close();
                }
            }

            return tasks;
        }


        public static List<string> get_tasks_for_today()
        {
            List<string> tasks = new List<string>();

            string mes = "при попытке получить данные о задачах на сегодня";

            if (check_access(mes))
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={path}"))
                {
                    db.Open();

                    string sql = $"SELECT {Planner.task} FROM {Tables.planner} WHERE {Planner.date} = '{Time.now_date()}'";

                    SqliteCommand command = new SqliteCommand(sql, db);

                    try
                    {
                        SqliteDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                            tasks.Add(reader.GetString(0));
                    }
                    catch (Exception ex)
                    {
                        string message = $"{mes} база данных вернула следующую ошибку: {ex.Message}";
                    }

                    db.Close();
                }
            }

            return tasks;
        }



        public static void update(string table, Dictionary<string, string> values, Dictionary<string, string> where)
        {
            string mes = "при попытке обновить данные в базе данных";

            if (check_access(mes))
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={path}"))
                {
                    db.Open();

                    SqliteCommand command = new SqliteCommand();

                    command.Connection = db;

                    string correspondence = field_value_string_for_update_row(values);
                    string sql = $"UPDATE {table} SET {correspondence} WHERE {where.ElementAt(0).Key} = '{where.ElementAt(0).Value}'";

                    command.CommandText = sql;

                    try
                    {
                        command.ExecuteReader();
                    }
                    catch (Exception ex)
                    {
                        string message = $"{mes} база данных вернула следующую ошибку: {ex.Message}";
                    }

                    db.Close();
                }
            }
        }

        private static string field_value_string_for_update_row(Dictionary<string, string> fields)
        {
            // в этот метод следует отправлять словарь с названиями полей и значениями для них, которые были проверены на существование и соответствие их типам
            // метод возвращает строку в формате "name = N'Михаил', surname = N'Евгениевич'"

            if (fields == null || fields.Count == 0)
            {
                string message = $"запрошена команда на формирование строки для команды создания таблица, онако входящий параметр Dictionary пуст или ==null." +
                                "процедура формирования строки прервана. строка не сформирована.";
            }

            string s = "";

            foreach (var item in fields)
            {
                s += $"{item.Key} = '{item.Value}', ";
            }

            s = s.Remove(s.Length - 1);
            s = s.Remove(s.Length - 1);

            return s;
        }

        public static void delete(string table, string field, string value)   // удаление строки из таблицы
        {
            string mes = "при попытке обновить данные в базе данных";

            if (check_access(mes))
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={path}"))
                {
                    db.Open();

                    SqliteCommand command = new SqliteCommand();

                    command.Connection = db;

                    string sql = $"DELETE FROM {table} WHERE {field} = '{value}'";

                    command.CommandText = sql;

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        string message = $"{mes} база данных вернула следующую ошибку: {ex.Message}";
                    }

                    db.Close();
                }
            }
        }

        public static void delete_task_from_today(string id)   // удаление задачи из списка на сегодня
        {
            string mes = "при попытке обновить данные в базе данных";

            if (check_access(mes))
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={path}"))
                {
                    db.Open();

                    SqliteCommand command = new SqliteCommand();

                    command.Connection = db;

                    string sql = $"DELETE FROM {Tables.planner} WHERE {Planner.task} = '{id}' AND {Planner.date} = '{Time.now_date()}'";

                    command.CommandText = sql;

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        string message = $"{mes} база данных вернула следующую ошибку: {ex.Message}";
                    }

                    db.Close();
                }
            }
        }


    }
}
