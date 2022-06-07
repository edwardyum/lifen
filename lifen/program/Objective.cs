using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace lifen
{
    public delegate void RefreshSubtasks();


    internal class Objective
    {
        //
        private readonly string id;

        //
        private string name = string.Empty;
        private string description = string.Empty;
        private bool   done = false;
        private string data_creation = string.Empty;
        private string data_completion = string.Empty;

        public bool added_for_today { get; set; } = false;

        //
        bool obtaining_data_from_db = false;
        public string Id { get { return id; } }
        public string DataCreation { get { return data_creation; } set { data_creation = value; } }
        public string Name { get { return name; } set { name = value; if (!obtaining_data_from_db) { set_name(); } } }
        public string Description { get { return description; } set { description = value; if (!obtaining_data_from_db) { set_description(); } } }
        public bool Done { get { return done; } set { done = value; if (!obtaining_data_from_db) { set_done(value); } } }
        public string DataCompletion { get { return data_completion; } set { data_completion = value; } }


        
        RefreshSubtasks refresh;    // это делагет этого уровня
        RefreshSubtasks refreshUp;  // это делега сверху
        public ObservableCollection<Objective> subtasks { get; set; } = new ObservableCollection<Objective>();

        // !!! внимание !!!     зависимые свойства не работают из-за того, что они static. это путает всю логику, а убрать static не получается, в этом случае не отрабатывает внутренний меанизм DependencyObject
        //public ObservableCollection<Objective> subtasks { get { return (ObservableCollection<Objective>)GetValue(subtasksProperty); } set { SetValue(subtasksProperty, value); } }
        //public static readonly DependencyProperty subtasksProperty = DependencyProperty.Register("subtasks", typeof(ObservableCollection<Objective>), typeof(Objective), new PropertyMetadata(new ObservableCollection<Objective>()));



        //
        private CommandBase add_task_command_;                 public ICommand add_task_command => add_task_command_;
        private CommandBase delete_task_command_;              public ICommand delete_task_command => delete_task_command_;
        private CommandBase add_task_for_today_command_;       public ICommand add_task_for_today_command => add_task_for_today_command_;
        private CommandBase delete_task_from_today_command_;   public ICommand delete_task_from_today_command => delete_task_from_today_command_;


        public Objective(string Id, RefreshSubtasks refreshSubtasks)
        {
            id = Id;
            refreshUp = refreshSubtasks;

            initialize();
            form();
        }


        public Objective(string Id, string name_, string description_, bool done_)
        {
            id = Id;
            name = name_;
            description = description_;
            done = done_;

            initialize();
        }

        private void initialize()
        {
            add_task_command_ = new CommandBase(add_task);
            delete_task_command_ = new CommandBase(delete_task);
            add_task_for_today_command_ = new CommandBase(add_task_for_today);
            delete_task_from_today_command_ = new CommandBase(delete_task_from_today);

            refresh = refreshSubtasks;
        }

        private void form()
        {
            DataTable data = SQLite.get_unic_row_with_condition_1(Tables.tasks, Tasks.Id, id);

            // id - уже есть
            data_creation =     data.Rows[0][Tasks.creation_date].ToString();
            name =              data.Rows[0][Tasks.name].ToString();
            description =       data.Rows[0][Tasks.description].ToString();
            done = Tools.string_to_bool(data.Rows[0][Tasks.done].ToString());
            data_completion =   data.Rows[0][Tasks.completion_date].ToString();

            refreshSubtasks();
        }

        private void refreshSubtasks()
        {
            // для снижения количества вычислений и обращений к базе данных будет проводить только обновление списка подзадач, а не его формирование заново
            // получаем списко id подзадач из базы данных и проверяем все ли задачи в текущем списке соответствуют списку id из базы данных
            // выбиаем каких подзадач нет и их добавляем. выбираем какие подзадачи лишние и их убираем.

            List<string> subId = get_subtasks_id();

            List<string> missing = missingSubtasks(subId);

            List<Objective> superfluous = superfluousSubtasks(subId);

            foreach (string sid in missing)
                subtasks.Add(new Objective(sid, refresh));

            foreach(var subtask in superfluous)
                subtasks.Remove(subtask);
        }

        private List<string> get_subtasks_id()
        {
            List<string> ids = SQLite.get_one_column(Tables.hierarchy, Hierachy.child, Hierachy.parent, id);
            return ids;
        }

        private List<string> missingSubtasks(List<string> subId)  // недостающие подзадачи
        {
            List<string> missing = new();

            foreach (string sid in subId)
            {
                bool exist = false;
                foreach (var task in subtasks)
                {
                    if (task.Id == sid)
                    {
                        exist = true;
                        break;
                    }
                }

                if(!exist)
                    missing.Add(sid);
            }
            return missing;
        }

        private List<Objective> superfluousSubtasks(List<string> subId)  // лишние подзадачи
        {
            List<Objective> superfluous = new();

            foreach (var task in subtasks)
            {
                bool exists = false;
                foreach (string sid in subId)
                {
                    if (task.Id == sid)
                    {
                        exists = true;
                        break;
                    }                        
                }

                if (!exists)
                    superfluous.Add(task);
            }
            return superfluous;
        }



        private Dictionary<string, string> this_task()
        {
            Dictionary<string, string> where = new Dictionary<string, string>();
            where.Add(Tasks.Id, id);
            return where;
        }


        public void add_task()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add(Tasks.name, "задача");
            values.Add(Tasks.creation_date, Time.now());
            values.Add(Tasks.done, Tools.bool_to_1_or_0(false));

            string row = SQLite.add(Tables.tasks, values);

            if (string.IsNullOrWhiteSpace(row))
            {
                string message = $"не удалось добавить новую строку для новой задачи в таблицу {Tables.tasks}";
            }

            Dictionary<string, string> hierarchy = new Dictionary<string, string>();
            hierarchy.Add(Hierachy.parent, id);
            hierarchy.Add(Hierachy.child, row);

            string row_project = SQLite.add(Tables.hierarchy, hierarchy);

            if (string.IsNullOrWhiteSpace(row_project))
            {
                string message = $"не удалось добавить новую строку для новой задачи в таблицу {Tables.hierarchy}";
            }

            refreshSubtasks();
        }

        public void add_task_for_today()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add(Planner.date, Time.now_date());
            value.Add(Planner.task, id);

            string row = SQLite.add(Tables.planner, value);

            if (string.IsNullOrWhiteSpace(row))
            {
                string message = $"не удалось добавить новую строку для новой задачи на сегодня в таблицу {Tables.planner}";
            }
        }

        public void delete_task()
        {
            if (subtasks != null)
                for (int i = 0; i < subtasks.Count; i++)
                    subtasks[i].delete_task();

            SQLite.delete(Tables.tasks, Tasks.Id, id);

            SQLite.delete(Tables.hierarchy, Hierachy.child, id);
            // удачная находнка - инкапсуляция. удаляем не по ссылке на родителя или какие-то другие объекты,
            // а все строки, в которые входит текущая задача. и убирается необходимость в поле id родителя.
            
            delete_task_from_today();   // лучше вызвать методв в классе Objective, чем в SQLite. более универсально.

            refreshSubtasks();

            if (refreshUp != null)
                refreshUp();
        }

        public void delete_task_from_today()
        {
            SQLite.delete_task_from_today(id);
        }


        

        public void set_description()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add(Tasks.description, description);

            Dictionary<string, string> where = this_task();

            SQLite.update(Tables.tasks, value, where);
        }

        public void set_done(bool haveDone)
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add(Tasks.done, Tools.bool_to_1_or_0(haveDone));
            value.Add(Tasks.completion_date, Time.now());

            Dictionary<string, string> where = this_task();

            SQLite.update(Tables.tasks, value, where);

            // пока отказываюсь от установления выполненными всех входящих подзадач
            ///////////////////////////////////////////////////////////////////////
            //if(subtasks!=null)
            //    foreach (Objective task in subtasks)
            //        task.set_done(haveDone);
        }

        public void set_name()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add(Tasks.name, name);

            Dictionary<string, string> where = this_task();

            SQLite.update(Tables.tasks, value, where);
        }




        // today

        public void check_for_today()
        {
            if (Manager.tasks_for_today.Contains(id))
                added_for_today = true;

            if (subtasks != null)
                for (int i = 0; i < subtasks.Count; i++)
                    subtasks[i].check_for_today();
        }

        public bool check_for_today_back()
        {
            bool today = false;

            if (subtasks != null)
                for (int i = 0; i < subtasks.Count; i++)
                {
                    if (subtasks[i].check_for_today_back())
                    {
                        today = true;
                        added_for_today = true;
                    }
                }

            if (added_for_today)
                today = true;

            return today;
        }

        public void create_today_only_structure()   // копируем корневой узел. удаляем всё, что не добавлено на сегодня
        {
            if (subtasks != null)
                for (int i = subtasks.Count; i >= 0; i--)
                    if (!subtasks[i].added_for_today) subtasks.RemoveAt(i);
                    else subtasks[i].create_today_only_structure();                    
        }


        // нет возможности добавить существующие объекты, поскольку они включают и не отмеченные на сегодня. если я удалю неотмеченные в today, они удаляться и в оригинале
        public void create_today_only_structure_2(Objective objective)  // создаём новый корневой узел. вставляем в него новые объекты с параметрами существующих
        {
            if (subtasks != null)
                foreach (Objective task in subtasks)
                    if (task.added_for_today)
                    {
                        Objective subtask = new Objective(task.id, task.name, task.description, task.done);
                        objective.subtasks.Add(subtask);
                        task.create_today_only_structure_2(subtask);
                    }
        }

    }
}
