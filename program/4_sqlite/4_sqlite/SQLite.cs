using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4_sqlite
{
    internal static class SQLite
    {
        public static void create_db(string location)
        {
            if (File.Exists(location))
            {
                string path = Path.GetDirectoryName(location);
                string name = Path.GetFileName(location);

                throw new Exception("предпринята попытка создать базу данных, однако, обнаружено, " +
                    $"что база данных с именем {name} уже существует в дирректории {path}");
            }
            else
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={location}"))
                {
                    db.Open();
                }
            }
        }

        public static void create_table(string location)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={location}"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT EXISTS tasks " +
                                      "(Id INTEGER PRIMARY KEY, name NVARCHAR(30) NOT NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }

    }
}
