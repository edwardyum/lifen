using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    // класс для всех видов проверок входных значений
    // соответствие типу
    // английская ли это строка или русская
    // содержит ли цифры или нет

    static class CheckSQLType
    {
        private static void check_null(object o)
        {
            if (o == null)
            {
                string message = $"запрошена команда проверки соответствия переменной типу. однако на вход пршёл пустой параметр" +
                                 "процедура проверки соответствия переменной типу прервана. тип не проверен.";
                Log.log(message);
                throw new ArgumentNullException(message);
            }
        }

        public static bool check(object o, SQLTypes sql_type, int size = 0)
        {
            check_null(o);

            bool matched = false;

            switch (sql_type)
            {
                case SQLTypes.BIT:
                    break;
                case SQLTypes.TINYINT:
                    break;
                case SQLTypes.INT:
                    matched = INT_check(o);
                    break;
                case SQLTypes.NVARCHAR:
                    matched = NVARCHAR_check(o, size);
                    break;
                case SQLTypes.DATETIME:
                    break;
                default:
                    break;
            }

            return matched;
        }

        // заготовка для метода проверки
        // может использовать с конструкцией switch внутри и создать для каждого случая свой метод
        // чтобы всё не прописывать

        private static bool _check(object o)
        {
            check_null(o);

            bool matched = false;

            TypeCode type = Type.GetTypeCode(o.GetType());

            if (type == TypeCode.String)
            {
                //...
            }
            else
            {
                matched = false;
            }

            return (matched);
        }


        //private bool BIT_check(object o)
        //{
        //    check_null(o);

        //    Type type_of_parameter = o.GetType();
        //    string type = type_of_parameter.Name;

        //    if (type == "Boolean" || type == "String")
        //    {

        //    }
        //}

        private static bool INT_check(object o)
        {
            check_null(o);

            bool matched = false;

            TypeCode type = Type.GetTypeCode(o.GetType());

            if (type == TypeCode.Int32)
            {
                    matched = true;
            }

            return (matched);
        }

        private static bool NVARCHAR_check(object o, int size = 0)
        {
            check_null(o);

            bool matched = false;

            TypeCode type = Type.GetTypeCode(o.GetType());

            if (type == TypeCode.String)
            {
                string s = o.ToString();
                if (s.Length <= size)
                    matched = true;
            }

            return (matched);
        }


        private static bool DATETIME_check(object o)
        {
            check_null(o);

            bool matched = false;

            TypeCode type = Type.GetTypeCode(o.GetType());

            if (type == TypeCode.String)
            {
                string s = o.ToString();

                DateTime dt = new DateTime();
                if (DateTime.TryParse(s, out dt))
                {
                    string s_date = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    if (s == s_date)
                    {
                        matched = true;
                    }
                }
                else
                {
                    matched = false;
                }
            }
            else
            {
                matched = false;
            }

            return (matched);
        }



        //switch (code)
        //    {
        //        case TypeCode.Empty:
        //            break;
        //        case TypeCode.Object:
        //            break;
        //        case TypeCode.DBNull:
        //            break;
        //        case TypeCode.Boolean:
        //            break;
        //        case TypeCode.Char:
        //            break;
        //        case TypeCode.SByte:
        //            break;
        //        case TypeCode.Byte:
        //            break;
        //        case TypeCode.Int16:
        //            break;
        //        case TypeCode.UInt16:
        //            break;
        //        case TypeCode.Int32:
        //            break;
        //        case TypeCode.UInt32:
        //            break;
        //        case TypeCode.Int64:
        //            break;
        //        case TypeCode.UInt64:
        //            break;
        //        case TypeCode.Single:
        //            break;
        //        case TypeCode.Double:
        //            break;
        //        case TypeCode.Decimal:
        //            break;
        //        case TypeCode.DateTime:
        //            break;
        //        case TypeCode.String:
        //            break;
        //        default:
        //            break;
        //    }

    }
}
