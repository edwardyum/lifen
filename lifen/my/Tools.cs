using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace lifen
{

    // ДОБАВИТЬ КЛАСС В БИБЛИОТЕКУ

    public static class Tools
    {
        public static string get_local_folder()
        {
            return ApplicationData.Current.LocalFolder.Path;
        }


        public static string bool_to_1_or_0(bool b)
        {
            return Convert.ToInt32(b).ToString();
        }

        public static bool string_to_bool(string s)
        {
            bool b = false;

            if (!string.IsNullOrWhiteSpace(s))
            {
                switch (s)
                {
                    case "true":
                        b = true; break;
                    case "True":
                        b = true; break;
                    case "TRUE":
                        b = true; break;

                    case "false":
                        b = false; break;
                    case "False":
                        b = false; break;
                    case "FALSE":
                        b = false; break;

                    case "1":
                        b = true; break;
                    case "0":
                        b = false; break;
                }
            }
            else
            {
                string message = "на вход в метод пришло пустое значение";
            }

            return b;
        }

        public static DataTable createDataTble(int r, int c)
        {
            DataTable dt = new DataTable();

            for (int i = 0; i < c; i++)
                dt.Columns.Add();

            for (int i = 0; i < r; i++)
                dt.Rows.Add();

            return dt;
        }

    }
}
