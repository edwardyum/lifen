using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Reflection;
using Windows.UI.Xaml;
using System.ComponentModel;

namespace lifen
{
    public delegate void RefreshToday(string id = null, RefreshType refreshType = RefreshType.Add);

    internal class ListsViewModel : DependencyObject
    {
        public static bool forming = false;

        public Objective root { get { return (Objective)GetValue(rootProperty); } set { SetValue(rootProperty, value); } }
        public static readonly DependencyProperty rootProperty = DependencyProperty.Register("root", typeof(Objective), typeof(ListsViewModel), new PropertyMetadata(null));


        public static List<Objective> tasks = new();
        public static List<Objective> todays = new();

        public static List<string> todayTasks;


        public ListsViewModel()
        {
            root = new Objective("1");
            formToday();
        }

        private void formToday()
        {
            // формируем на сегодня не из списка задач на сегодня todays, а из базы данных. согласно моему принципу отсутствия дублирования информации
            
            // получаем список всех задач на сегодня
            // получаем список всех веток
            // обращаемся к каждому узлу ветки и добавляем в today нужную подзадачу из subdivisions

            todayTasks = SQLite.get_column(Tables.planner, Planner.task, Planner.date, Time.now_date());

            foreach (var idt in todayTasks)
                addBranchForNode(idt);
        }

        public static void addBranchForNode(string id)    // для указанного узла находим ветку и все её узлы добавляем на сегодня
        {
            List<string> nodes = branch(id);

            for (int i = 0; i < nodes.Count; i++)
            {
                Objective t = tasks.Find(x => x.Id == nodes[i]); //если перенесём t.Added_for_today = true; в цикл, то исключим добавление корневого узла "1" в задачи на сегодня при каждом вызове

                if (i < nodes.Count - 1)
                {
                    Objective st = t.subtasks.Single(x => x.Id == nodes[i + 1]);
                    st.added_for_today = true;
                    if (!t.today.Contains(st))
                        t.today.Add(st);
                }
            }
        }

        public static void addToday(string id)       // добавить задачу на сегодня
        {
            addBranchForNode(id);
        }

        public static void addTodayAndSubtasks(string id)       // добавить задачу на сегодня и все подзадачи на всех уровнях
        {
            // получаем все подадачи
            // для каждой подзадачи вызываем этот метод
            // добавляем подзадачу в список на сегодня
        }

        public static void excludeToday(string id)   // убрать задачу из списка на сегодня
        {
            // бежим обратно по ветке и исключаем узлы из задач на сегодня
            // если встречаем узел содержит другие задачи на сегодня прекращаем процесс исключения

            List<string> nodes = branch(id);

            for (int i = nodes.Count-2; i >= 0; i--)
            {
                Objective task = tasks.Find(x => x.Id == nodes[i]);
                
                Objective subTask = tasks.Find(x => x.Id == nodes[i + 1]);
                subTask.added_for_today = false;

                task.today.Remove(subTask);

                if (task.today.Count != 0)
                    break;
            }
        }

        public static void excludeTodayAndSubtasks(string id)       // исключить задачу на сегодня и все подзадачи на всех уровнях
        {
            // получаем все подадачи
            // для каждой подзадачи вызываем этот метод
            // в конце метода исключаем подзадачу из списка на сегодня
        }

        public static List<string> branch(string id)
        {
            // если пришёл запрос на поиск из корня - возвращаем пустой список
            if (id == "1")
                return new List<string>();

            // считаем, что задача принадлежит только одной надзадаче
            List<string> nodes = new() { id};
            do
            {
                id = SQLite.get_unic_cell_with_condition(Tables.hierarchy, Hierachy.parent, Hierachy.child, id);
                nodes.Add(id);
            } while (id != "1");

            nodes.Reverse();

            return nodes;
        }

        public static Objective getParent(string idChild)
        {
            string  idp = SQLite.get_unic_cell_with_condition(Tables.hierarchy, Hierachy.parent, Hierachy.child, idChild);
            Objective parent = tasks.Find(x=>x.Id == idp);
            return parent;
        }

    }
}
