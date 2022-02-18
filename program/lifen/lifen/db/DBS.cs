using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{
    internal static class DBS
    {
        private static string connectionString = "";
        public static SqlConnection connection = null;

        public static void initialize()
        {
            connectionString = Global.connction_string;
        }

        public static void open()
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

        public static void close()
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

    }
}