using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{
    internal static class Global
    {
        
        public static string connction_string = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=C:\Users\Эдуард\Desktop\Новая папка (2)\lifen-main\program\1_db\db\db.mdf";
        //public static string connction_string = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=C:\Users\Эдуард\Desktop\Новая папка (2)\lifen-main\program\1_db\db\db.mdf;Integrated Security=SSPI;Trusted_Connection=True";

        //public static string connction_string = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Эдуард\Desktop\Новая папка (2)\lifen-main\program\1_db\db\db.mdf;Integrated Security=SSPI;Trusted_Connection=True";
        //public static string connction_string = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\e.yumagulov\Desktop\системы\lifen\program\1_db\db\db.mdf;Integrated Security=True,";


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
