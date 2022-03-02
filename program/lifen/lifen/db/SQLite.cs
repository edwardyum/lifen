using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{

    // ДОБАВИТЬ КЛАСС В БИБЛИОТЕКУ

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

                    string tableCommand = "CREATE TABLE IF NOT EXISTS MyTable " +
                                          "(Primary_Key INTEGER PRIMARY KEY, Text_Entry NVARCHAR(2048) NULL)";

                    SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                    createTable.ExecuteReader();
                }
            }
        }

        

    }
}
