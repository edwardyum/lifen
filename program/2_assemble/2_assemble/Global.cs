using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_assemble
{
    internal static class Global
    {

        // ЕСЛИ СДЕЛАТЬ СВОЙСТВОМ, ТО ПРИ ВЫЗОВЕ НЕ ПОТРЕБУЕТСЯ ПИСАТЬ СКОБКИ :)
        public static string date_for_writing_in_db()
        {
            // при отправке даты в базу анных в формате MM.dd.yyyy база данных изменяет порядок на dd.MM.yyyy

            string date = DateTime.Now.Date.ToString("MM.dd.yyyy");
            return date;
        }

        // ЕСЛИ СДЕЛАТЬ СВОЙСТВОМ, ТО ПРИ ВЫЗОВЕ НЕ ПОТРЕБУЕТСЯ ПИСАТЬ СКОБКИ :)
        public static string date_for_reading_from_db()
        {
            // дата в базе данных лежит в формате dd.MM.yyyy обращаться к ней стоит в формате 'yyyy.MM.dd' - с одинарными кавычками            

            string date = "'" + DateTime.Now.Date.ToString("yyyy.MM.dd") + "'";
            return date;
        }

        // ЕСЛИ СДЕЛАТЬ СВОЙСТВОМ, ТО ПРИ ВЫЗОВЕ НЕ ПОТРЕБУЕТСЯ ПИСАТЬ СКОБКИ :)
        public static string datetime_for_writing_in_db()
        {
            // при отправке даты в базу анных в формате "yyyy-MM-dd HH:mm:ss.fff" база данных изменяет порядок на "dd.MM.yyyy HH:mm:ss"

            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return date;
        }


    }
}
