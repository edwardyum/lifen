using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{
    internal static class Log
    {
        public static ObservableCollection<string> logs = new ObservableCollection<string>();
        private static object locker = new object();

        public static int elements_in_listbox = 30;          // количество элементов показываемых на форме - чтобы небыло особо много. оптимизация. и не было потенциального переполнения. безопасность.
        public static int elements_in_collection = 100;      // количество элементов показываемых в списке logs - чтобы не было потенциального переполнения. безопасность.
        public static ListBoxFlow flow = ListBoxFlow.Down;

        private static StreamWriter sw /*= new StreamWriter(Global.path_to_log_file, true, System.Text.Encoding.Default)*/;


        public static void initialize()
        {
            // важно как можно быстрее устанавливать sw.AutoFlush = true; Сейчас делаю это при создании объекта manager
            // устанавливает автоматическую запись буфера в файл. сначала всё, что есть записывается в буфер. потом в файл.
            sw.AutoFlush = true;
        }


        public static void log(string s)
        {
            lock (locker)
            {
                logs.Add(s);
                //sw.WriteLine(s);

                //if (logs.Count > elements_in_collection + 10)
                //    for (int i = 0; i < 10; i++)
                //        logs.RemoveAt(0);
            }
        }

        //static event LogHandler LogHandlerEvent;

        //public static void assign_handler(LogHandler handler)
        //{
        //    logs.CollectionChanged += LogHandlerEvent;
        //}

        // !!!
        // ПЛОХАЯ ПРОИЗВОДИТЕЛЬНОСТЬ. ПОЛУЧАЕТСЯ НА КАЖДОЕ ИЗМЕНЕНИЕ logs ВЫПОЛНЯЕТСЯ ЭТОТ БОЛЬШОЙ МЕТОД.
        // СЛЕДУЕТ ПРИДУМАТЬ ЧТО-ТО ДРУГОЕ.
        // ПРИ КАЖДОМ ИЗМЕНЕНИИ logs МЕТОД ОТОБРАЖАЮЩИЙ ЛОИГИ ОБРАЩАЕТСЯ К ПОЛУЧЕНИЮ СПИСКА ЛОГОВ

        public static string[] get_logs(int n)
        {
            int length = Math.Min(logs.Count, n);
            string[] ss = new string[length];

            //if (logs.Count > 20)
            //{
            //    int a = 2;
            //}

            lock (locker)
            {
                int k = 0;
                for (int i = logs.Count - 1; i >= logs.Count - length; i--)
                {
                    ss[k] = logs[i];
                    k++;
                }

            }




            return ss;
        }

        public static string[] get_logs_reverse(int n)
        {
            int length = Math.Min(logs.Count, n);
            string[] ss = new string[length];

            lock (locker)
            {
                for (int i = logs.Count - length; i < logs.Count; i++)
                    ss[i] = logs[i];
            }

            return ss;
        }

        static void write_in_log_file()
        {

        }





        static string add_time()
        {
            string required_time = "";

            DateTime time = DateTime.Now;

            string hours = time.Hour.ToString();
            string minutes = time.Minute.ToString();
            string seconds = time.Second.ToString();
            string millisec = time.Millisecond.ToString();

            required_time = $"в {hours} часов, {minutes} минут, {seconds} секунд и {millisec} миллисекунд";

            return required_time;
        }


        //// !!!
        //// в Form

        //// 1.
        //// в загрузчик формы
        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    Log.logs.CollectionChanged += show_new_log;
        //    Control.CheckForIllegalCrossThreadCalls = false;
        //}

        //// 2.
        //// вставить в форму - обработчик для ListBox

        //void show_new_log(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    int allowed_count = Log.elements_in_listbox;

        //    switch (Log.flow)
        //    {
        //        case ListBoxFlow.Up:
        //            listBox1.Items.Add(Log.logs.Last());
        //            clear_ListBox();
        //            break;

        //        case ListBoxFlow.Down:
        //            listBox1.Items.Insert(0, Log.logs.Last());
        //            clear_ListBox();
        //            break;

        //        default:
        //            break;
        //    }
        //}

        //void clear_ListBox()  // при достижении количества записей в ListBox дозволенного количества старые удаляются
        //{
        //    int allowed_count = Log.elements_in_listbox;

        //    switch (Log.flow)
        //    {
        //        case ListBoxFlow.Up:
        //            if (listBox1.Items.Count == allowed_count + 1)
        //                listBox1.Items.RemoveAt(0);

        //            if (listBox1.Items.Count > allowed_count)
        //                for (int i = 0; i < listBox1.Items.Count - allowed_count; i++)
        //                    listBox1.Items.RemoveAt(0);
        //            break;

        //        case ListBoxFlow.Down:
        //            if (listBox1.Items.Count == allowed_count + 1)
        //                listBox1.Items.RemoveAt(listBox1.Items.Count - 1);

        //            if (listBox1.Items.Count > allowed_count)
        //                for (int i = 0; i < listBox1.Items.Count - allowed_count; i++)
        //                    listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
        //            break;

        //        default:
        //            break;
        //    }
        //}




        //"произошла чудовищная ошибка: "
    }
}
