using System;

public class Class1
{
	public static Class1()
	{
        public static void InitializeDatabase()
        {
            //await ApplicationData.Current.LocalFolder.CreateFileAsync("sqliteSample.db", CreationCollisionOption.OpenIfExists);
            //string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteSample.db");
            string dbpath = @"C:\Users\e.yumagulov\AppData\Local\Packages\f5a53da5-17be-49c0-8ecd-463b1460ced1_2wpxf91wdknyr\LocalState\sqliteSample.db";
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY, " +
                    "Text_Entry NVARCHAR(2048) NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }
    }
}
