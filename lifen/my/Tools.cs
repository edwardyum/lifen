using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static string deleteLastWord(string s, string last)
        {
            char[] chars = last.ToCharArray();
            s = s.TrimEnd(chars);

            return s;
        }


        public static ObservableCollection<DateTime> daysSetFromBeginingOfEyar()    // список дней с начала года до текущего дня. первый день выбирается как первый день недели, в которую начался год.
        {
            ObservableCollection<DateTime> days = new();

            string begin = $"{DateTime.Now.Year.ToString()}.01.01";
            DateTime initial = DateTime.Parse(begin);

            int weekDay = (int)initial.DayOfWeek;               // перед началом года - для того, чтобы набор начинался с понедельника
            initial = initial.AddDays(-weekDay + 1);

            do                                                  // начиная с начала года
            {
                days.Add(initial);
                initial = initial.AddDays(1);

            } while ((DateTime.Now - initial).Days > 0);

            days.Add(initial);

            return days;
        }

    }
}
