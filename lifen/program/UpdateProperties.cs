using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{
    // функционал отражает принцип, лежащйи в основе программы. отсутствие дублирования информации для предотвращения расхождения что в базе данных и что в переменной
    // при изменении свойства в модели, сначала записываем в базу данных, потом считываем что записали, а не присваиваем переменной сразу новое значение.

    // правильным будет вынести функционал в отдельный класс, поскольку он
    // может присутствовать в нескольких классах и является отдельной сущьностью - предоставляет сгруппированные по смыслу функции

    // кроме того позволяет
    // добавлять новую информацию в базу данных - новые записи
    // удалять существующую информацию из базы данных

    internal static class UpdateProperties
    {
        public static void set(object o, string name, object value, string table, string where, string condition, string column = null)
        {
            // o - объект класса, в котором расположено обрабатываемое свойство / поле
            // forming = (WithTeacher.forming) - при начальной загрузке программы формируются все объекты и поля. для предотвращения второго цикла записи в базу данных вводится этот флаг
            // если не задан столбец, подразумевается, что его имя совпадает с именем поля

            // для поулчения закрыты полей следует установить дополнительные настройки. без них privat поля не будут возвращаться методом GetField.
            // должны быть указаны два флага: NonPublic - для поиска среди закртытых полей, и Instance - для поиска нестатических полей

            Type type = o.GetType();
            FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);

            if (ListsViewModel.forming)
                field.SetValue(o, value);
            else
            {
                if (column == null)
                    column = field.Name;

                update(table, column, value.ToString(), where, condition);      // запись в базу данных

                string val = read(table, column, where, condition);             // чтение из базы данных
                object var = Convert.ChangeType(val, field.FieldType);
                field.SetValue(o, var);
            }
        }

        private static void update(string table, string column, string value, string where_column, string where_condition)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add(column, value);

            Dictionary<string, string> where = new Dictionary<string, string>();
            where.Add(where_column, where_condition);

            SQLite.update(table, values, where);
        }

        private static string read(string table, string column, string where, string condition)
        {
            string value = SQLite.get_unic_cell_with_condition(table, column, where, condition);
            return value;
        }



    }
}
