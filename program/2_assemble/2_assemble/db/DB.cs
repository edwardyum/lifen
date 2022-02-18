using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_assemble
{
    // внимание
    // существует два класса, работающие с базой данных DBsql и DB
    // DBsql работает с базой данных на основе SQL запросов
    // DB работает с базой данных на основе метода Update объекта класса SqlDataAdapter
    // функционал разнесён по разделения функционала
    // однако все функции подключения и отключения от базы данных общие


    internal class DB
    {
        // правила
        // весь функционал работы с базой данных на основе SQL запросов помещать в класс DBsql

        // вся часть связанная с подключением к базе данных, отключением, проверки подключения и переподключения
        // полностью заимствована без изменений из статического класса DBsql
        // при необходимости внесения изменений в эту часть следует внести изменения в статическом классе и скопировать их сюда


        public DataSet ds_auto = new DataSet();
        public DataTable ColumnsNameType = new DataTable();

        private string connectionString = "";
        public SqlConnection connection = null;

        private SqlDataAdapter adapter = null;


        public DB(string ConnectionString)
        {
            Initialize(ConnectionString);
        }

        public DB()
        {

        }

        public void Initialize(string ConnectionString)
        {
            connectionString = ConnectionString;
        }


        /// Connection

        public void open()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                Log.log("запрошена команда на подключение к базе данных, однако обнаружено, что база данных уже подключена\n" +
                        "процесс подключения базы данных прерваан" +
                        "если переподключение необходимо запросите команду переподключения к базе данных");
                return;
            }

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrWhiteSpace(connectionString))
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

        public void close()
        {
            if (connection == null)
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

        public bool check_connection()
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

        public void reconnect()
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


        public DataTable get_table_auto(string table)
        {
            // в DataSet создаётся таблца, которой присваивается тоже имя: table
            // на события обновления строк, добавления и удаления подписываются соответствующие методы 
            // формируется таблица названиями столбцов и их типами для использования пр проверках соответствия типам

            if (!DBsql.table_is_exists(table))
            {
                string message = $"запрошена команда на получение таблицы из базе данных, однако таблицы с таким именем [{table}] нет в базе данных." +
                                 "процедура получения таблицы прервана. таблица не получена.";
                Log.log(message);
                throw new Exception(message);
            }

            string sql = $"SELECT * FROM [{table}]";

            adapter = new SqlDataAdapter(sql, connection);

            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            builder.GetInsertCommand();
            builder.GetUpdateCommand();
            builder.GetDeleteCommand();

            try
            {
                Log.log($"выполняется sql команда: {sql}");
                adapter.Fill(ds_auto, table);
            }
            catch (Exception ex)
            {
                Log.log($"ошибка при выполнении команды получения таблицы {table}: {ex.Message}");
            }

            ds_auto.Tables[table].RowChanged += RowChangeHandler;
            ds_auto.Tables[table].RowDeleted += RowChangeHandler;
            ds_auto.Tables[table].TableNewRow += RowNew;

            ColumnsNameType = DBsql.get_fields_and_types_of_table(table);

            return ds_auto.Tables[0];
        }

        private void RowNew(object sender, DataTableNewRowEventArgs e)
        {
            string table = e.Row.Table.TableName;
            update_table(table);
        }

        private void RowChangeHandler(object sender, DataRowChangeEventArgs e)
        {
            // при обновлении метод вызывается два раза - сначала при внесении изменений в значения строки. изменённая строка получает статус Modified
            // после этого вносятся изменения в базу данных и метод Update помечает строку Unchanged.
            // поскольку произошло изменение статуса, то это воспринимается как изменение строки и второй раз вызывается этот метод
            // однако доходя при достижении метода Update ничего не происходит, поскольку статус везде Unchanged. на это процесс останавливается.

            string table = e.Row.Table.TableName;
            update_table(table);
        }

        public void update_table(string table)
        {
            // автоматическое без явного sql запроса создание, удаление и обновление записей в таблице в базе данных
            try
            {
                adapter.Update(ds_auto, table);
            }
            catch (Exception ex)
            {
                Log.log($"ошибка при выполнении команды автоматического обновления таблицы {table}: {ex.Message}");
            }
        }

        public void update_all_tables()
        {
            // автоматическое без явного sql запроса создание, удаление и обновление записей во всех таблицах в базе данных

            // может появиться ошибка Update unable to find TableMapping['Table'] or DataTable 'Table'.
            // это ошибка в библиотеке microsoft. она связана с наличием/отсутствием в DataSet таблицы с именем 'Table'
            // https://stackoverflow.com/questions/19228608/dataadapter-update-unable-to-find-tablemappingtable-or-datatable-table

            try
            {
                adapter.Update(ds_auto);
            }
            catch (Exception ex)
            {
                Log.log($"ошибка при выполнении команды автоматического обновления всех таблиц: {ex.Message}");
            }
        }

    }
}
