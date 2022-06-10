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
        private bool done = false;
        private string data_creation = string.Empty;
        private string data_completion = string.Empty;

        private bool added_for_today = false;
        public bool Added_for_today { get { return added_for_today; } set { if (value) add_task_for_today(); else delete_task_from_today(); } }

        //
        public string Id { get { return id; } }
        public string DataCreation { get { return data_creation; } set { setup(nameof(data_creation), value, Tasks.creation_date); } }
        public string Name { get { return name; } set { setup(nameof(name), value); } }
        public string Description { get { return description; } set { setup(nameof(description), value); } }
        public bool Done { get { return done; } set { setup(nameof(done), value); DataCompletion = Time.now(); } }
        public string DataCompletion { get { return data_completion; } set { setup(nameof(data_completion), value, Tasks.completion_date); } }



        RefreshSubtasks refresh;    // делагет этого уровня
        RefreshSubtasks refreshUp;  // делегат сверху
        RefreshToday refreshToday;  // обновление задач на сегодня
        public ObservableCollection<Objective> subtasks { get; set; } = new ObservableCollection<Objective>();

        // !!! внимание !!!     зависимые свойства не работают из-за того, что они static. это путает всю рекурсивную логику , а убрать static не получается, в этом случае не отрабатывает внутренний меанизм DependencyObject
        //public ObservableCollection<Objective> subtasks { get { return (ObservableCollection<Objective>)GetValue(subtasksProperty); } set { SetValue(subtasksProperty, value); } }
        //public static readonly DependencyProperty subtasksProperty = DependencyProperty.Register("subtasks", typeof(ObservableCollection<Objective>), typeof(Objective), new PropertyMetadata(new ObservableCollection<Objective>()));



        //
        private CommandBase add_task_command_; public ICommand add_task_command => add_task_command_;
        private CommandBase delete_task_command_; public ICommand delete_task_command => delete_task_command_;
        //private CommandBase add_task_for_today_command_;       public ICommand add_task_for_today_command => add_task_for_today_command_;
        //private CommandBase delete_task_from_today_command_;   public ICommand delete_task_from_today_command => delete_task_from_today_command_;

        // для root
        public Objective(string Id, RefreshSubtasks refreshSubtasks, RefreshToday refreshToday)
        {
            id = Id;
            refreshUp = refreshSubtasks;
            this.refreshToday = refreshToday;

            initialize();
            form();
        }


        // для today
        public Objective(string Id_, RefreshToday refreshToday)
        {
            id = Id_;
            this.refreshToday = refreshToday;
            formBase();
        }

        private void initialize()
        {
            add_task_command_ = new CommandBase(add_task);
            delete_task_command_ = new CommandBase(delete_task);
            //add_task_for_today_command_ = new CommandBase(add_task_for_today);
            //delete_task_from_today_command_ = new CommandBase(delete_task_from_today);

            refresh = refreshSubtasks;
        }


        private void setup(string field, object value, string column = null)
        {
            UpdateProperties.set(this, field, value, Tables.tasks, Tasks.Id, id, column);
        }

        //private void setupForToday(bool value)
        //{
        //    switch (value)
        //    {
        //        case true:
        //            add_task_for_today(); break;

        //        case false:
        //            delete_task_from_today(); break;
        //    }
        //}

        private void form()
        {
            formBase();
            refreshSubtasks();            

            // функционал по определению назначена ли задача на сегодня выполняется во ViewModel
            //if (ListsViewModel.todayTasks.Contains(Id))
            //    added_for_today = true;
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
                subtasks.Add(new Objective(sid, refresh, refreshToday));

            foreach (var subtask in superfluous)
                subtasks.Remove(subtask);
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
                    refreshToday(id);
                }
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
            Dictionary<string, string> where = new();
            where.Add(Planner.task, id);
            where.Add(Planner.date, Time.now_date());

            SQLite.delete(Tables.planner, where);


            refreshToday(Id, RefreshType.Delete);

            //SQLite.delete_task_from_today(id);
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



        //////////////////////////////////////////////////
        // today

        public void check_for_today()
        {
            if (ListsViewModel.todayTasks.Contains(id))
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

        //public void create_today_only_structure()   // копируем корневой узел. удаляем всё, что не добавлено на сегодня
        //{
        //    if (subtasks != null)
        //        for (int i = subtasks.Count; i >= 0; i--)
        //            if (!subtasks[i].added_for_today) subtasks.RemoveAt(i);
        //            else subtasks[i].create_today_only_structure();                    
        //}


        // нет возможности добавить существующие объекты, поскольку они включают и не отмеченные на сегодня. если я удалю неотмеченные в today, они удаляться и в оригинале
        public void create_today_only_structure_2(Objective objective)  // создаём новый корневой узел. вставляем в него новые объекты с параметрами существующих
        {
            if (subtasks != null)
                foreach (Objective task in subtasks)
                    if (task.added_for_today)
                    {
                        //Objective subtask = new Objective(task.id, task.name, task.description, task.done);
                        //objective.subtasks.Add(subtask);
                        //task.create_today_only_structure_2(subtask);
                    }
        }


        // new approach
        //public void wolkAlongBranch(List<string> ids, int level)
        //{
        //    level++;
        //    added_for_today = true;

        //    if (level != ids.Count - 1) // если это не последний уровень, добавляем подзадачу
        //    {
        //        string subtaskId = ids[level + 1];

        //        if (!subtasksContain(subtaskId))
        //            subtasks.Add(new Objective(subtaskId, refreshToday));

        //        var subtask = getSubtaskById(subtaskId);
        //        subtask.wolkAlongBranch(ids, level);
        //    }
        //}


        //public enum ThereAndBack
        //{
        //    There,
        //    Back
        //}

        //public static ThereAndBack thereBack = ThereAndBack.There;

        //public static bool thereAndBack = true;    // true - ход в глубину рекурсии, false - обратный ход
        public static bool deletion = true;

        public void wolkAlongBranchThereAndBack(List<string> ids, int level, RefreshType refreshType)
        {
            // идём туда и обратно по ветке
            // если задача установить новую задачу на сегодня,
            //      то по пути туда добавляем все необходимые узлы, если они ещё не добавлены
            //      по пути обратно ничего не делаем
            // если задача удалить существующую задачу на сегодня,
            //      то по пути туда ничего не делаем. если обнаруживаем, что части ветки нет, возвращаемся
            //      по пути назад удаляем задачи

            // в обратном ходе следует обратить внимание одну особенность.
            // с одной стороны можно удалить задачу на уровень ниже, но она помимо удалённой по заказу подзадачи может содержать другие назначенные на сегодня
            // и можно было бы удалитьвсю подзадачу, но это удалит и требуетмые подподзадачи
            // пользователь может вызвать не из самого конца удаление, а например, задачу, а подзадачи могут остаться.
            // это допустимо. всё равно удаляем её и идём всё вверх. правдо надо на уровен root удостовериться в удалении подподзадач из сегодня
            // тогда условие удаления: если задача включает и другие подзадачи, то следует сделат отметку, что удаление закончено и просто возвращаемся            

            // используется приём с return в середине. когда доходи до последнего элемента, то возвращаемся и продолжаем выполнять действия на обратном ходу.
            // такое не сделать с enum, поскольку ветка упадёт только в один вариант, а второго - обратного - не достигнет. она уже попала в первый и нет резона выходить в другие
            // можно сделать с двойным if, но это тоже самое, что не делать это

            level++;
            string subtaskId = string.Empty;

            if (level != ids.Count - 1) // если это не последний уровень, добавляем подзадачу
            {
                subtaskId = ids[level + 1];

                if (!subtasksContain(subtaskId))
                    if (refreshType == RefreshType.Add)
                    {
                        added_for_today = true;
                        subtasks.Add(new Objective(subtaskId, refreshToday));
                    }

                var subtask = getSubtaskById(subtaskId);
                subtask.wolkAlongBranchThereAndBack(ids, level, refreshType);
            }
            else
                return;

            ////////// обратный ход //////////

            if (refreshType == RefreshType.Delete)
                if (deletion)
                {
                    if (subtasksContain(subtaskId))
                    {
                        var subtask = getSubtaskById(subtaskId);
                        subtasks.Remove(subtask);
                    }
                    if (subtasks.Count > 0)
                        deletion = false;
                }
        }

        private bool subtasksContain(string id_)    // это subtasks в today, а не в root
        {
            bool contain = false;

            foreach(Objective task in subtasks)
                if (task.Id==id_)
                {
                    contain = true;
                    break;
                }

            return contain;
        }

        private Objective getSubtaskById(string id_)
        {
            Objective task = null;

            foreach (var subtask in subtasks)
                if (subtask.Id == id_)
                {
                    task = subtask;
                    break;
                }            

            return task;
        }

        private bool getSubtaskByIdIfExist(string id_, out Objective task)
        {
            bool contain = false;
            task = null;

            foreach (var subtask in subtasks)
                if (subtask.Id == id_)
                {
                    contain = true;
                    task = subtask;
                    break;
                }

            return contain;
        }
    }
}
