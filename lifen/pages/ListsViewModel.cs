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

        public Objective today { get { return (Objective)GetValue(todayProperty); } set { SetValue(todayProperty, value); } }
        public static readonly DependencyProperty todayProperty = DependencyProperty.Register("today", typeof(Objective), typeof(ListsViewModel), new PropertyMetadata(null));

        public static List<string> todayTasks;

        RefreshToday refreshToday;  // работает только для добавления новой задачи на сегодня. пока отсутствует функционал удаления задачи из сегодня. для удаления используется полное обновление


        public ListsViewModel()
        {
            refreshToday = todayRefresh;
            root = new Objective("1", null, refreshToday);  // корневой узел

            today = new Objective("1", refreshToday);
            formToday();
        }

        private void formToday()
        {
            today.subtasks.Clear();
            todayTasks = SQLite.get_column(Tables.planner, Planner.task, Planner.date, Time.now_date());
            foreach (var idt in todayTasks)
                todayRefresh(idt);
        }

        // либо добавляем новую ветку, либо полностью обновляем все ветки
        // это сделано ввиду относительно большой трудоёмкости создания метода удаления одной задачи на сегодня
        private void todayRefresh(string id, RefreshType refreshType = RefreshType.Add)
        {
            switch (refreshType)
            {
                case RefreshType.Add:
                    walk(id, refreshType);
                    break;

                case RefreshType.Delete:
                    walk(id, refreshType);
                    break;

                case RefreshType.Total:
                    formToday();
                    break;
            }
        }

        private void walk(string id, RefreshType refreshType)
        {
            List<string> ids = branch(id);
            int level = -1;

            Objective.deletion = true;
            today.wolkAlongBranchThereAndBack(ids, level, refreshType);
            Objective.deletion = true;
        }


        public static List<string> branch(string id)
        {
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



    }
}
