using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;

using System.Windows;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace _2_assemble
{
    internal class MainWindowViewModel : DependencyObject
    {
        //public List<Project> projects { get; set; } = new List<Project>();

        private readonly CommandBase new_project_command_;
        public ICommand new_project_command => new_project_command_;

        private readonly CommandBase refresh_command_;
        public ICommand refresh_command => refresh_command_;


        public ObservableCollection<Project> projects
        {
            get { return (ObservableCollection<Project>)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("projects", typeof(ObservableCollection<Project>), typeof(MainWindowViewModel), new PropertyMetadata(new ObservableCollection<Project>()));

        //ObservableCollection<Project> projects_today { get; set; };



        public ObservableCollection<Project> projects_today
        {
            get { return (ObservableCollection<Project>)GetValue(projects_todayProperty); }
            set { SetValue(projects_todayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for projects_today.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty projects_todayProperty =
            DependencyProperty.Register("projects_today", typeof(ObservableCollection<Project>), typeof(MainWindowViewModel), new PropertyMetadata(new ObservableCollection<Project>()));



        public MainWindowViewModel()
        {
            new_project_command_ = new CommandBase(new_project);

            refresh_command_ = new CommandBase(refresh);


            Manager.refresh_event += Manager_refresh_event;

            Manager.execute();
        }

        private void Manager_refresh_event()
        {
            projects.Clear();

            for (int i = 0; i < Manager.projects.Count; i++)
                projects.Add(Manager.projects[i]);


            projects_today.Clear();

            for (int i = 0; i < Manager.today.projects.Count; i++)
                projects_today.Add(Manager.today.projects[i]);
        }

        private void refresh()
        {
            Manager.execute();
        }

        private void new_project()
        {
            // добавляем новую строку в таблицу task
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("name", "новый проект");

            DBsql.insert_row_in_table_sql(Manager.table_tasks, values);

            // добавляем новую строку в таблицу иерархии
            DataTable last_row = DBsql.get_last_row_in_table(Manager.table_tasks);

            string id_of_new_row_in_task = last_row.Rows[0]["Id"].ToString();

            Dictionary<string, string> values_project = new Dictionary<string, string>();
            values_project.Add("parent", "1");      // 1 = root
            values_project.Add("child", id_of_new_row_in_task);

            DBsql.insert_row_in_table_sql(Manager.table_id, values_project);

            Manager.execute();
        }


    }
}
