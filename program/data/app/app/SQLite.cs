using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace app
{
    internal static class SQLite
    {
        private static string db_name = "db.db";
        private static string path_to_folder;
        private static string path;                 // полный путь к базе данных

        public static void initialize()
        {
            path_to_folder = Tools.get_local_folder();
            path = Path.Combine(path_to_folder, db_name);
        }

        public async static void create_db()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(db_name, CreationCollisionOption.OpenIfExists);
        }

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


        public static void AddData(string inputText)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={path}"))
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                string sql = "INSERT INTO MyTable VALUES (NULL, @Entry);";

                command.CommandText = sql;
                command.Parameters.AddWithValue("@Entry", inputText);

                command.ExecuteReader();

                db.Close();
            }
        }


        public static List<string> get_data()
        {
            List<string> entries = new List<string>();

            using (SqliteConnection db = new SqliteConnection($"Filename={path}"))
            {
                db.Open();

                string sql = "SELECT Text_Entry from MyTable";

                SqliteCommand command = new SqliteCommand(sql, db);

                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    entries.Add(reader.GetString(0));
                }

                db.Close();
            }

            return entries;
        }


    }
}
