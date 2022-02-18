using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    // внимание
    // существует два класса, работающие с базой данных DBsql и DB
    // DBsql работает с базой данных на основе SQL запросов
    // DB работает с базой данных на основе метода Update объекта класса SqlDataAdapter
    // функционал разнесён по разделения функционала
    // однако все функции подключения и отключения от базы данных общие

    static class DBsql
    {
        // правила

        // весь функционал работы с базой данных на основе SQL запросов помещать в класс DBsql

        // 2
        // при создании и внесении чего-то нового в базу данных всё проверяется на соответствие требованиям
        // - требования к имена
        // - соответствие типам полей
        // при обращении к базе данных проверяется наличие к чему обращается в базе данных
        // - поля в таблице
        // в одном методе может быть и обращени и создание, например при внесении изменений в строку 


        // +
        // ВВЕСТИ ПРОВЕРКУ НАЗВАНИЯ
        // - ТАБЛИЦ
        // - ПОЛЕЙ
        // НА ПРАВИЛЬНОСТЬ
        // - НА АНГЛИЙСКОМ
        // - В ОДНО СЛОВО
        // - БЕЗ ЛИШНИХ СИМВОЛОВ. ТОЛЬКО БУКВЫ И/ИЛИ ЦИФРЫ И/ИЛИ НИЖНЕЕ ПОДЧЁРКИВАНИЕ
        // ПРЕДУПРЕДИТЬ, ЧТО МОЖНО И БОЛЬШЕ, НО НЕ НУЖНО
        // ПЕРЕДЕЛАТЬ МЕТОД СОЗДАНИЯ ТАБЛИЦЫ (И ДРУГИЕ) В ЧАСТИ НОВЫХ ТРЕБОВАНИЙ К ПОЛЯМ И ТАБЛИЦАМ

        // +
        // ПРИСУТСТВУЕТ ПРОВЕРКА НА ПУСТУЮ СТРОКУ. ЗАМЕНИТЬ НА СВОЙ МЕТОД ИЗ СВОЕЙ БИБЛИОТЕКИ.

        // +
        // получение полей таблицы и их типов

        // ВВЕСТИ ПРОВЕРКУ НА СУЩЕСТВОВАНИЕ ПОЛЕЙ, К КОТОРЫМ ОБРАЩАЕМСЯ В ТАБЛИЦЕ

        // +
        // ПРИ СОЗДАНИИ ТАБЛИЦЫ НАСТРОИТЬ ПЕРВЫЙ СТОЛБЕЦ - КЛЮЧЕВОЙ ИНДЕКС УВЕЛИЧИВАЮЩИЙСЯ НА 1

        // ЧАСТЬ ПРОВЕРОК МОЖНО ВЫНЕСТИ НАРУЖУ. МОЖЕТ БУДЕТ ПРОЩЕ И БОЛЕЕ ПРАВИЛЬНО.
        // УЧЕСТЬ N'' для значений в методе field_type_string_for_table_creation
        // сделать механизм работы с типами полей (хотя бы основными) на основе перечисления

        // вынести в отдельный метод определения латиницы из check_name_for_db()
        // сделать метод определения кириллицы

        // СДЕЛАТЬ ПЕРЕЧЕНЬ ВЕЩЕЙ КОТОРЫЕ СЛЕДУЕТ УЧИТЫВАТЬ ПРИ РАЗРАБОТКЕ МЕТОДОВ, РАБОТАЮЩИХ СО СТРОКАМИ
        // ПРОБЕЛЫ, ТАБЫ, ENTER, ВСЁ ОСТАЛЬНОЕ
        // В КАКОМ ПОРЯДКЕ ОНИ МОГУТ ИДТИ ДРУГ ЗА ДРУГОМ

        // переделать метод создания таблицы на введение в том числе ключевого поля и указания, что оно ключевое
        // сделать перегрузку для введения без ключевого поля. тогда автоматом появляется поле id
        // учесть при этом все проверки полей и типов. и чтобы во втором случае не было другого поля Id.
        // добавить простановку возможности поля быть null. убрать у ключевого поля.

        // +
        // сделать поле connection снова закрытым private

        // ЗАПОМНИТЬ
        // метод string.IsNullOrWhiteSpace() учитывает большое количество знаков разделения, включая перенос строки
        // я проверил



        private static string connectionString = "";
        public static SqlConnection connection = null;

        public static void InitializeComponent(string ConnectionString)
        {
            connectionString = ConnectionString;
        }



        /// String

        public static bool check_name_for_db(string name)   // проверка имени таблицы или поля на соответствие
        {
            // мои требования жёстче официальных. можно их расширить, но не нужно.
            
            // требования
            // * не пустая строка +
            // * начинается только с буквы или "_", не может начинаться с цифры +
            // * количество букв не более 10 (это не обзязательно, но желательно) +
            // * количество слов равно строго 1. +
            // ** это требование автоматически выполняется при удовлетворении следующему требованию +
            // * состоит только +
            // ** из латиницы +
            // ** цифр +
            // ** знака "_" +


            // решил сделать метод сквозной. не прерывать на первой ошибке, чтобы все проблемы подсветились.
            // кроме первой проверки. если она не выполняется, то остальные соответственно все проваливаются.
            // и это упрощает дальнейшую логику - не надо проверять на ноль в каждом случае

            bool fine = true;

            if (string.IsNullOrWhiteSpace(name))
            {
                Log.log("метод проверки имени таблицы или поля: имя пустое или содержит только символы разделителей");
                return false;                
            }

            if (Char.IsDigit(name[0]))
            {
                Log.log("метод проверки имени таблицы или поля: имя начинается с цифры");
                fine = false;
            }
                        
            if (name.Length > 10)
            {
                Log.log("метод проверки имени таблицы или поля: длинна имени больше 10 символов");
                fine = false;
            }

            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || Char.IsDigit(c) || name[i] == '_'))
                {
                    Log.log($"метод проверки имени таблицы или поля: имя содержит запрещённый символ: '{c}'");
                    fine = false;
                }
            }

            if (fine)
                Log.log($"метод проверки имени таблицы или поля: имя успешно прошло проверку и удовлетворения всем требованиям");

            return fine;
        }

        private static string field_type_string_for_table_creation(List<Tuple<string,string>> fields)
        {
            if (fields == null || fields.Count == 0)
            {
                string message = $"запрошена команда на формирование строки для команды создания таблица, онако входящий параметр Dictionary пуст или ==null." +
                                "процедура формирования строки прервана. строка не сформирована.";
                Log.log(message);
                throw new Exception(message);
            }


            string s = "";

            for (int i = 0; i < fields.Count; i++)
            {
                var item = fields[i];
                if (!check_name_for_db(item.Item1))
                {
                    string message = $"при формировании строки для создания таблицы обнаружено, " +
                                     $"что минимум одно из полей [{item.Item1}] не удовлетворяет требованиям, предъявляемым к названиям полей." +
                                     "процедура формирования строки прервана. строка не сформирована.";
                    Log.log(message);
                    throw new Exception(message);
                }

                s += $"{item.Item1} {item.Item2}, ";
            }

            s = s.Remove(s.Length - 1);
            s = s.Remove(s.Length - 1);

            return s;
        }

        private static string field_value_string_for_update_row (Dictionary<string, string> fields)
        {
            // в этот метод следует отправлять словарь с названиями полей и значениями для них, которые были проверены на существование и соответствие их типам
            // метод возвращает строку в формате "name = N'Михаил', surname = N'Евгениевич'"

            if (fields == null || fields.Count == 0)
            {
                string message = $"запрошена команда на формирование строки для команды создания таблица, онако входящий параметр Dictionary пуст или ==null." +
                                "процедура формирования строки прервана. строка не сформирована.";
                Log.log(message);
                throw new Exception(message);
            }

            string s = "";

            foreach (var item in fields)
            {
                s += $"{item.Key} = N'{item.Value}', ";
            }

            s = s.Remove(s.Length - 1);
            s = s.Remove(s.Length - 1);

            return s;
        }

        private static Tuple<string,string> fields_values_strings_for_row(Dictionary<string, string> fields)
        {
            // в этот метод следует отправлять словарь с названиями полей и значениями для них, которые были проверены на существование и соответствие их типам
            // метод выдаёт строку полей в формате "name, surname, birthday"
            // метод выдаёт строку значений в формате "N'андрей', N'андреев', N'12/09/2021'"


            if (fields == null || fields.Count == 0)
            {
                string message = $"запрошена команда на формирование строк для работы со строками в таблице в базе данных, онако входящий параметр Dictionary пуст или ==null." +
                                "процедура формирования строк прервана. строки не сформированы.";
                Log.log(message);
                throw new Exception(message);
            }

            
            string f = "", v = "";

            foreach (var item in fields)
            {
                f += $"{item.Key}, ";
                v += $"N'{item.Value}', ";
            }

            f = f.Remove(f.Length - 1);
            f = f.Remove(f.Length - 1);
            v = v.Remove(v.Length - 1);
            v = v.Remove(v.Length - 1);


            Tuple<string, string> strings = new Tuple<string, string>(f,v);

            return strings;
        }

        //public static int words_in_string(string s) // количество слов в строке
        //{
        //    // ПЕРЕДЕЛАТЬ. УЧЕСТЬ ВСЕ ВОЗМОЖНЫЕ ВЕЩИ
        //    string[] vs = s.Split();

        //    return vs.Length;
        //}



        /// Connection

        public static void open()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                Log.log("запрошена команда на подключение к базе данных, однако обнаружено, что база данных уже подключена\n" +
                        "процесс подключения базы данных прерваан" +
                        "если переподключение необходимо запросите команду переподключения к базе данных");
                return;
            }

            if ( string.IsNullOrEmpty(connectionString ) || string.IsNullOrWhiteSpace(connectionString))
            {
                Log.log("строка подключения к базе данных пустая или = null.\n" +
                        "процесс подключения базы данных прерван.");
                return;
            }

            connection = new SqlConnection(connectionString);
            connection.Open();

            if (connection.State == ConnectionState.Open)
                Log.log("подключение к базе данных установлено");
            else
                Log.log("не удалось подключиться к базе данных");
        }

        public static void close()
        {
            if (connection==null)
            {
                Log.log("запрошена команда на закрытие отключения к базе данных, но обнаружено, что объект подключения == null.\n" +
                        "процесс закрытия подключения к базе данных прерваан.");
                return;
            }

            try
            {
                connection.Close();
            }
            catch (Exception ex)
            {
                Log.log($"при попытке закрыть подключение к базе данных возникла ошибка: {ex.Message}");
            }

            if (connection.State == ConnectionState.Closed)
                Log.log("подключение к базе данных закрыто");
            else
                Log.log("не удалось закрыть подключение к базе данных");
        }

        public static bool check_connection()
        {
            if (connection == null)
            {
                string message = "запрошена команда на проверку подключения к базе данных, но обнаружено, что объект подключения == null";
                Log.log(message);

                throw new Exception(message);
            }

            bool connected = false;

            if (connection.State == ConnectionState.Open)
            {
                connected = true;
                Log.log("подключение к базе данных установлено");
            }
            else
            {
                connected = false;
                Log.log("нет подключения к базе данных");
            }
                
            return connected;
        }

        public static void reconnect()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }

            connection = null;
            connection = new SqlConnection(connectionString);

            connection.Open();

            if (connection.State == ConnectionState.Open)
                Log.log("успешно выполнено переподключение к базе данных");
            else
                Log.log("не удалось переподключиться к базе данных");
        }



        /// Tables
        
        public static bool table_is_exists(string table)
        {
            if (!check_connection())
            {
                string message = "запрошена команда на проверку существования таблицы в базе данных, однако нет подключения к базе данных" +
                                 "процедура проверки существования таблицы в базе данных прервана";
                Log.log(message);
                throw new Exception(message);
            }

            bool exists = false;

            string sql = $"IF OBJECT_ID(N'dbo.[{table}]', N'U') IS NOT NULL SELECT 1 AS res ELSE SELECT 0 AS res";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
            DataSet data = new DataSet();

            try
            {
                Log.log($"выполняется sql команда: {sql}");
                adapter.Fill(data);
            }
            catch (Exception ex)
            {
                Log.log($"ошибка при выполнения команды sql проверки существования таблицы: {ex.Message}");
            }
            

            exists = data.Tables[0].Rows[0][0].ToString() == "1";         

            return exists;
        }

        public static DataTable get_fields_and_types_of_table(string table)
        {
            if (!table_is_exists(table))
            {
                string message = $"запрошена команда на получение имён и типов полей таблицы в базе данных, однако таблицы с таким именем [{table}] нет в базе данных." +
                                 "процедура получения имён и типов полей таблицы прервана. имена и типы полей не получены.";
                Log.log(message);
                throw new Exception(message);
            }

            string sql = $"SELECT column_name, data_type FROM information_schema.columns WHERE table_name = N'{table}'";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
            DataSet data = new DataSet();

            try
            {
                Log.log($"выполняется sql команда: {sql}");
                adapter.Fill(data);
            }
            catch (Exception ex)
            {
                Log.log($"ошибка при выполнения команды sql получения имён и типов полей таблицы: {ex.Message}");
            }

            return data.Tables[0];
        }

        public static void create_table(string table, List<Tuple<string, string>> fields)
        {
            // помимо требуемых пользователем полей метод автоматически подставляет первое поле id, увеличивающееся на 1
            // [Id]                INT IDENTITY(1, 1) NOT NULL,
            // а также добавляет в конец sql запроса требование сделать это поле ключевым
            // PRIMARY KEY CLUSTERED ([Id] ASC)


            if (!check_connection())
            {
                string message = "запрошена команда на создание новой таблицы в базе данных, однако нет подключения к базе данных" +
                                 "процедура создания таблицы прервана. таблица не создана.";
                Log.log(message);
                throw new Exception(message);
            }

            if (table_is_exists(table))
            {
                string message = $"запрошена команда на создание новой таблицы в базе данных, однако таблица с таким именем [{table}] уже есть в базе данных." +
                                 "процедура создания таблицы прервана. таблица не создана.";
                Log.log(message);
                throw new Exception(message);
            }

            if (!check_name_for_db(table))
            {
                string message = $"запрошена команда на создание новой таблицы в базе данных, однако имя таблицы [{table}] не удовлетворяет требованиям." +
                                 "процедура создания таблицы прервана. таблица не создана.";
                Log.log(message);
                throw new Exception(message);
            }


            fields.Insert(0, new Tuple<string, string>("Id", "INT IDENTITY(1, 1) NOT NULL"));
            string field_type = field_type_string_for_table_creation(fields);
            
            string key_annotation = "PRIMARY KEY CLUSTERED ([Id] ASC)";

            string sql = $"CREATE TABLE {table} ({field_type}, {key_annotation})";

            SqlCommand cmd = new SqlCommand(sql, connection);

            try
            {
                Log.log($"выполняется sql команда: {sql}");
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.log($"ошибка при выполнении кодманды создания таблицы {table}: {ex.Message}");
            }
            
            if (table_is_exists(table))
                Log.log($"выполнена команда создания таблицы {table}. проверка показала, что таблица успешно создана");
            else
                Log.log($"выполнена команда создания таблицы {table}. однако проверка показала, что таблица не была создана");
        }
        
        public static void delete_table(string table)
        {
            // не применяю проверку имени таблицы. предполагатся, что такая таблица уже существует и требуется только её удалить
            // проверка имени таблицы только при её создании. это исключает ошибки (если с базой данных работаю только я).            


            if (!check_connection())
            {
                string message = $"запрошена команда на удаление таблицы {table} из базы данных, однако нет подключения к базе данных" +
                                 "процедура удаления таблицы прервана. таблица не удалена.";
                Log.log(message);
                throw new Exception(message);
            }

            if (!table_is_exists(table))
            {
                string message = $"запрошена команда на удаление таблицы {table} из базы данных, однако таблицы с таким именем [{table}] нет в базе данных." +
                                 "процедура удаления таблицы прервана. таблица не удалена.";
                Log.log(message);
                throw new Exception(message);
            }


            string sql = $"DROP TABLE {table}";

            SqlCommand cmd = new SqlCommand(sql, connection);

            try
            {
                Log.log($"выполняется sql команда: {sql}");
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.log($"ошибка при выполнении кодманды удаления таблицы {table}: {ex.Message}");
            }

            if (!table_is_exists(table))
                Log.log($"выполнена команда удаления таблицы {table}. проверка показала, что таблица успешно удалена");
            else
                Log.log($"выполнена команда удаления таблицы {table}. однако проверка показала, что таблица не была удалена");
        }

        public static DataTable get_table(string table)
        {
            if (!table_is_exists(table))
            {
                string message = $"запрошена команда на получение таблицы из базе данных, однако таблицы с таким именем [{table}] нет в базе данных." +
                                 "процедура получения таблицы прервана. таблица не получена.";
                Log.log(message);
                throw new Exception(message);
            }

            string sql = $"SELECT * FROM [{table}]";
            
            SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
            DataSet data = new DataSet();

            try
            {
                Log.log($"выполняется sql команда: {sql}");
                adapter.Fill(data);
            }
            catch (Exception ex)
            {
                Log.log($"ошибка при выполнении кодманды получения таблицы {table}: {ex.Message}");
            }            

            return data.Tables[0];
        }



        /// Rows

        public static void insert_row_in_table_sql(string table, Dictionary<string, string> values)
        {
            // ПРОВЕРИТЬ СУЩЕСТВОВАНИЕ ПОЛЕЙ И СООТВЕТСТВИЕ ИХ ТИПУ

            if (!table_is_exists(table))
            {
                string message = $"запрошена команда на вставку строки в таблицу в базе данных, однако таблицы с таким именем [{table}] нет в базе данных." +
                                 "процедура вставки строки в таблицу прервана. строка не вставлена.";
                Log.log(message);
                throw new Exception(message);
            }
            

            Tuple<string,string> strings = fields_values_strings_for_row(values);
            string sql = $"INSERT INTO {table} ({strings.Item1}) VALUES ({strings.Item2})";

            SqlCommand cmd = new SqlCommand(sql, connection);

            try
            {
                Log.log($"выполняется sql команда: {sql}");
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.log($"ошибка при выполнении кодманды вставки новой строки в таблицу {table}: {ex.Message}");
            }
        }

        public static void delete_row_in_table_sql(string table, string field, string value)
        {
            // ПРОВЕРИТЬ СУЩЕСТВОВАНИЕ ПОЛЕЙ И СООТВЕТСТВИЕ ИХ ТИПУ

            if (!table_is_exists(table))
            {
                string message = $"запрошена команда на удаление строки из таблицы в базе данных, однако таблицы с таким именем [{table}] нет в базе данных." +
                                 "процедура удаления строки из таблицы прервана. строка не удалена.";
                Log.log(message);
                throw new Exception(message);
            }

            string sql = $"DELETE FROM {table} WHERE {field} = N'{value}'";

            SqlCommand cmd = new SqlCommand(sql, connection);

            try
            {
                Log.log($"выполняется sql команда: {sql}");
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.log($"ошибка при выполнении кодманды удаления строки из таблицы {table}: {ex.Message}");
            }
        }

        public static void update_row_in_table_sql(string table, Dictionary<string, string> values, Dictionary<string, string> where)
        {
            if (values == null || values.Count == 0)
            {
                string message = $"запрошена команда на редактирование строки в таблице в базе данных, однако входящий параметр со значениями либо = null либо пуст" +
                                 "процедура редактирования строки в таблицы прервана. изменения в строку не внесены.";
                Log.log(message);
                throw new Exception(message);
            }

            if (where == null || where.Count != 1)
            {
                string message = $"запрошена команда на редактирование строки в таблице в базе данных, однако входящий параметр с условием либо = null либо в нём не 1 элемент" +
                                 "процедура редактирования строки в таблицы прервана. изменения в строку не внесены.";
                Log.log(message);
                throw new Exception(message);
            }

            if (!table_is_exists(table))
            {
                string message = $"запрошена команда на редактирование строки в таблице в базе данных, однако таблицы с таким именем [{table}] нет в базе данных." +
                                 "процедура редактирования строки в таблицы прервана. изменения в строку не внесены.";
                Log.log(message);
                throw new Exception(message);
            }

            // проверка существования полей и соответствие их типу

            DataTable dt = get_fields_and_types_of_table(table);

            if (check_names_of_fields(dt, values))
            {
                string message = $"проверка показала, что отправляемые в базу данных поля отсутствуют в таблице в базе данных.";
                Log.log(message);
                throw new Exception(message);
            }

            string correspondence = field_value_string_for_update_row(values);
            string sql = $"UPDATE {table} SET {correspondence} WHERE {where.ElementAt(0).Key} = N'{where.ElementAt(0).Value}'";

            SqlCommand cmd = new SqlCommand(sql, connection);

            try
            {
                Log.log($"выполняется sql команда: {sql}");
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.log($"ошибка при выполнении кодманды редактирования строки в таблице {table}: {ex.Message}");
            }            
        }

        private static bool check_names_of_fields(DataTable in_db, Dictionary <string,string> to_db)
        {
            string exception_res = "процедура проверки соответствия имён столбцов в базе данных и отправляемых в базу данных прервана. соответствие не подтверждено.";

            if (in_db == null || in_db.Rows.Count==0)
            {
                string message = $"запрошена команда на проверку соответствия имён столбцов в базе данных и отправляемых в базу данных, однако таблица с именами полей на входе была пустой или = null." + exception_res;
                Log.log(message);
                throw new ArgumentNullException(message);
            }

            if (to_db == null || to_db.Count == 0)
            {
                string message = $"запрошена команда на проверку соответствия имён столбцов в базе данных и отправляемых в базу данных, однако словарь с именами отправляемых полей на входе была пустой или = null." + exception_res;
                Log.log(message);
                throw new ArgumentNullException(message);
            }

            bool matched = true;

            if (to_db.Count > in_db.Rows.Count)
            {
                matched = false;

                string message = $"количество отправляемых столбцов больше количества столбцов, содержащихся в базе данных";
                Log.log(message);
            }

            string [] in_dn_names = new string [in_db.Rows.Count];

            foreach (var item in to_db)
            {
                if (!in_dn_names.Contains(item.Key))
                {
                    matched = false;
                    Log.log($"имя поля, отправляемого в таблицу: {item.Key} не содержится в списке полей таблицы в базе данных.");
                }
            }

            return matched;
        }

    }
}
