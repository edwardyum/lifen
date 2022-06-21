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
    internal class Objective
    {
        //
        private readonly string id;

        //
        private string name = string.Empty;
        private string description = string.Empty;
        private bool done = false;
        private string data_creation = string.Empty;
        private string data_completion = string.Empty;

        public bool added_for_today = false;
        public bool Added_for_today { get { return added_for_today; } set { if (value) add_task_for_today(); else delete_task_from_today(); } }

        //
        public string Id { get { return id; } }
        public string DataCreation { get { return data_creation; } set { setup(nameof(data_creation), value, Tasks.creation_date); } }
        public string Name { get { return name; } set { setup(nameof(name), value); } }
        public string Description { get { return description; } set { setup(nameof(description), value); } }
        public bool Done { get { return done; } set { setup(nameof(done), value); DataCompletion = Time.now(); } }
        public string DataCompletion { get { return data_completion; } set { setup(nameof(data_completion), value, Tasks.completion_date); } }


        public ObservableCollection<Objective> subtasks { get; set; } = new ObservableCollection<Objective>();

        // !!! внимание !!!     зависимые свойства не работают из-за того, что они static. это путает всю рекурсивную логику , а убрать static не получается, в этом случае не отрабатывает внутренний меанизм DependencyObject
        //public ObservableCollection<Objective> subtasks { get { return (ObservableCollection<Objective>)GetValue(subtasksProperty); } set { SetValue(subtasksProperty, value); } }
        //public static readonly DependencyProperty subtasksProperty = DependencyProperty.Register("subtasks", typeof(ObservableCollection<Objective>), typeof(Objective), new PropertyMetadata(new ObservableCollection<Objective>()));

        public ObservableCollection<Objective> today { get; set; } = new ObservableCollection<Objective>();

        //
        private CommandBase add_task_command_; public ICommand add_task_command => add_task_command_;
        private CommandBase delete_task_command_; public ICommand delete_task_command => delete_task_command_;



        public Objective(string Id)
        {
            id = Id;

            initialize();
            form();

            if(!ListsViewModel.tasks.Contains(this))
                ListsViewModel.tasks.Add(this);

            if (Added_for_today)
                if (!ListsViewModel.todays.Contains(this))
                    ListsViewModel.todays.Add(this);
        }


        private void initialize()
        {
            add_task_command_ = new CommandBase(add_task);
            delete_task_command_ = new CommandBase(delete_task);
        }


        private void setup(string field, object value, string column = null)
        {
            UpdateProperties.set(this, field, value, Tables.tasks, Tasks.Id, id, column);
        }


        private void form()
        {
            formBase();
            refreshSubtasks();
        }

        private void formBase()
        {
            DataTable data = SQLite.get_unic_row_with_condition_1(Tables.tasks, Tasks.Id, id);

            // id - уже есть
            data_creation = data.Rows[0][Tasks.creation_date].ToString();
            name = data.Rows[0][Tasks.name].ToString();
            description = data.Rows[0][Tasks.description].ToString();
            done = Tools.string_to_bool(data.Rows[0][Tasks.done].ToString());
            data_completion = data.Rows[0][Tasks.completion_date].ToString();

            if (todayExists())
                added_for_today = true;
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
            {
                Objective subtask = new Objective(sid);
                subtasks.Add(subtask);

                if(subtask.Added_for_today)
                    today.Add(subtask);
            }                

            foreach (var subtask in superfluous)
            {
                subtasks.Remove(subtask);
                //today.Remove(subtask);
            }
        }

        private List<string> get_subtasks_id()
        {
            List<string> ids = SQLite.get_column(Tables.hierarchy, Hierachy.child, Hierachy.parent, id);
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

                if (!exist)
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

        public void delete_task()
        {
            // !!! внимание !!! важна последовательность действий. в методе выполняется несколько действий. удаление задачи из списка задач и из таблицы иерархий, из списка на сегодня и обновление родтельской задачи
            // однако при удалении из списка иерархии не могут выполниться удаление на сегодня и обновление родителя.
            // поэтому сначала удаляем из списка на сегодня, возможно в будущем, если сделаю удаление задач на сегодня через обновление списков, как в subtasks, то это будет не актуально и можно будет в любом порядке.
            // потом получаем объект родителя, потом удаляем задачу на сегодня, потом обновляем родителя. иначе родителя не получить - таблица иерархии уже не имеет нужной записи.
            

            // удаляем из списков на сегодня
            delete_task_from_today();


            Objective parent = ListsViewModel.getParent(Id);

            // удаляем из базы данных
            if (subtasks != null)
                for (int i = 0; i < subtasks.Count; i++)
                    subtasks[i].delete_task();

            SQLite.delete(Tables.tasks, Tasks.Id, id);

            SQLite.delete(Tables.hierarchy, Hierachy.child, id);
            // удачная находнка - инкапсуляция. удаляем не по ссылке на родителя или какие-то другие объекты,
            // а все строки, в которые входит текущая задача. удаляется у всех надзадач её включающих. + убирается необходимость в поле id родителя.


            // обновляем
            refreshSubtasks();
            parent.refreshSubtasks();
        }




        //////////////////////////////////////////////////
        // today

        // new lists today approach 

        private bool todayExists()
        {
            Dictionary<string, string> whereE = new();
            whereE.Add(Planner.task, id);
            whereE.Add(Planner.date, Time.now_date());

            bool exists = SQLite.exists(Tables.planner, Planner.task, whereE);

            return exists;
        }

        public void add_task_for_today()
        {
            if (!todayExists()) // проверка добавлена ли уже задача на сегодня
            {
                Dictionary<string, string> value = new Dictionary<string, string>();
                value.Add(Planner.date, Time.now_date());
                value.Add(Planner.task, id);

                string row = SQLite.add(Tables.planner, value);

                if (string.IsNullOrWhiteSpace(row))
                {
                    string message = $"не удалось добавить новую строку для новой задачи на сегодня в таблицу {Tables.planner}";
                }
                else
                {
                    ListsViewModel.addToday(Id);
                }
            }
        }

        public void delete_task_from_today()
        {
            Dictionary<string, string> where = new();
            where.Add(Planner.task, id);
            where.Add(Planner.date, Time.now_date());

            SQLite.delete(Tables.planner, where);

            ListsViewModel.excludeToday(Id);
        }


    }
}
