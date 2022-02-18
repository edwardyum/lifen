using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace db
{
    public partial class Form1 : Form
    {
        string connction_string = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\e.yumagulov\Desktop\системы\program\program\1_db\db\db.mdf;Integrated Security=True";
        //private SqlConnection connection = null;

        DataSet db = new DataSet();
        
        bool flag = false;

        public Form1()
        {
            InitializeComponent();

            // логирование
            Log.logs.CollectionChanged += show_new_log;
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //DB.InitializeComponent(connction_string);
            //DB.open();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DBsql.close();
            }
            catch (Exception)
            {

            }
        }

        void show_new_log(object sender, NotifyCollectionChangedEventArgs e)
        {
            int allowed_count = Log.elements_in_listbox;

            switch (Log.flow)
            {
                case ListBoxFlow.Up:
                    //listBox1.Items.Add(Log.logs.Last());listBox1.Items.AddRange()
                    clear_ListBox();
                    break;

                case ListBoxFlow.Down:
                    //listBox1.Items.Insert(0, Log.logs.Last());
                    listBox1.Items.Clear();
                    listBox1.Items.AddRange(Log.get_logs(Log.elements_in_listbox));
                    clear_ListBox();
                    break;

                default:
                    break;
            }
        }

        void clear_ListBox()
        {
            int allowed_count = Log.elements_in_listbox;

            switch (Log.flow)
            {
                case ListBoxFlow.Up:
                    if (listBox1.Items.Count == allowed_count + 1)
                        listBox1.Items.RemoveAt(0);

                    if (listBox1.Items.Count > allowed_count)
                        for (int i = 0; i < listBox1.Items.Count - allowed_count; i++)
                            listBox1.Items.RemoveAt(0);
                    break;

                case ListBoxFlow.Down:
                    if (listBox1.Items.Count == allowed_count + 1)
                        listBox1.Items.RemoveAt(listBox1.Items.Count - 1);

                    if (listBox1.Items.Count > allowed_count)
                        for (int i = 0; i < listBox1.Items.Count - allowed_count; i++)
                            listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                    break;

                default:
                    break;
            }
        }



        void get_tasks_from_db()
        {
            //string sql = "SELECT * FROM tasks";

            //adapter = new SqlDataAdapter(sql, connection);

            //SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            //builder.GetInsertCommand();
            //builder.GetUpdateCommand();
            //builder.GetDeleteCommand();

            //adapter.Fill(db, "tasks");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            get_tasks_from_db();
            dataGridView1.DataSource = db.Tables["tasks"];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string table = "task";

            List<Tuple<string, string>> fields = new List<Tuple<string, string>>();
            fields.Add(new Tuple<string, string>("column1", "int"));
            fields.Add(new Tuple<string, string>("column2", "nvarchar(20)"));

            try
            {
                DBsql.create_table(table, fields);
            }
            catch (Exception)
            {

            }            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string table = "task";

            try
            {
                DBsql.delete_table(table);
            }
            catch (Exception)
            {

            }            
        }

        private void button4_Click(object sender, EventArgs e)  //test
        {
            test_1();
        }

        private void test_1()
        {
            string name = textBox1.Text;
            //name = "\n\t\n\n\n\n\t\t";

            DBsql.check_name_for_db(name);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = DBsql.get_table("task");
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                Log.log(ex.Message);
            }            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string table = "task";
            Dictionary<string, string> fields = new Dictionary<string, string>();
            fields.Add("column1", "2");
            fields.Add("column2", "фыв");

            try
            {
                DBsql.insert_row_in_table_sql(table, fields);
            }
            catch (Exception)
            {

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string table = "task";

            try
            {
                DBsql.delete_row_in_table_sql(table, "column2", "???");
            }
            catch (Exception)
            {

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string table = "task";

            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("column1", "7");
            values.Add("column2", "€к 4t data");

            Dictionary<string, string> where = new Dictionary<string, string>();
            where.Add("column2", "фыв");

            try
            {
                DBsql.update_row_in_table_sql(table, values, where);
            }
            catch (Exception)
            {

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string table = "task";

            try
            {
                DBsql.get_fields_and_types_of_table(table);
            }
            catch (Exception)
            {

            }
        }

        string table = "tasks";
        DB dB = null;

        private void button10_Click(object sender, EventArgs e)
        {
            ///////////////////////////////////////////////
            /// объект DB
            dB = new DB(connction_string);
            dB.open();

            DBsql.connection = dB.connection;

            flag = false;
            dataGridView1.DataSource = dB.get_table_auto(table);
            flag = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string table = "for_test";

            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("column1", "1");
            value.Add("column2", "1");
            value.Add("column3", "1");
            value.Add("column4", "1");
            value.Add("column5", "1");

            Dictionary<string, string> where = new Dictionary<string, string>();
            where.Add("Id", "1");

            DBsql.InitializeComponent(connction_string);
            DBsql.open();
            DBsql.update_row_in_table_sql(table, value, where);
            DBsql.close();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            
            if (r > (dB.ds_auto.Tables[table].Rows.Count - 1) || c > (dB.ds_auto.Tables[table].Columns.Count - 1))
            {
                string message = $"индекс изменЄнной €чейки выходит за границы размера таблицы." +
                                 $"операци€ внесени€ изменений в таблицу прервана. изменени€ не внесены в таблицу.";
                Log.log(message);
                throw new Exception(message);
            }

            // внимание!!! сначала номер столбца, потом номер строки в dataGridView
            string s = dataGridView1[c, r].Value.ToString();

            dB.ds_auto.Tables[table].Rows[r][c] = s;

            dB.update_table(table);            
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // при начальной загрузке таблицы пропускаем еЄ, ничего не делаем
            //if (flag)
            //{
            //    //dB.ds_auto.Tables[table].ImportRow(((DataRow)dataGridView1.Rows[e.RowIndex]));
            //    //DataTable dt = (DataTable)dataGridView1.DataSource;
            //    //DataRow dataRow = dB.ds_auto.Tables[table].NewRow();

            //    //for (int i = 0; i < dataGridView1.Columns.Count; i++)
            //    //{
            //    //    dataRow[i] = dataGridView1[dataGridView1.Columns.Count - 1, e.RowIndex];
            //    //}

            //    dB.ds_auto.Tables[table].Rows.Add();


            //    //dB.ds_auto.Tables[table].ImportRow(dt.Rows[e.RowIndex]);
            //    //dB.ds_auto.Tables[table].ImportRow(((DataTable)dataGridView1.DataSource).Rows[e.RowIndex]);

            //    //dB.ds_auto.Tables[table].Rows.Add(dataGridView1.Rows[e.RowIndex]);
            //}
        }

        private void button12_Click(object sender, EventArgs e)
        {
            DBsql.InitializeComponent(connction_string);
            DBsql.open();


            string table_id = "hierarchy";
            string table_tasks = "tasks";
            // получение имЄн и id проектов
            // 1 - root
            //string sql_names = $"SELECT child FROM {table} WHERE parent = 1";
            string sql_names = $"SELECT {table_id}.child AS 'Id', {table_tasks}.name FROM {table_id}, {table_tasks} WHERE {table_id}.parent = 1 AND {table_id}.child = {table_tasks}.Id";
            //string sql = $"SELECT * FROM [{table}]";

            SqlDataAdapter adapter = new SqlDataAdapter(sql_names, DBsql.connection);
            DataSet data = new DataSet();
            
            adapter.Fill(data,"projects");

            DataTable projects = data.Tables["projects"];

            // получение таблиц проектов
            string sql_content = "";

            for (int i = 0; i < projects.Rows.Count; i++)
            {
                //sql_content = $"SELECT child FROM {table_id} where parent = {projects.Rows[i][0]}";
                sql_content = $"SELECT * FROM {table_tasks}, {table_id} WHERE parent = {projects.Rows[i][0]} AND {table_id}.child = {table_tasks}.Id";
                SqlDataAdapter adapter_content = new SqlDataAdapter(sql_content, DBsql.connection);
                adapter_content.Fill(data, projects.Rows[i][1].ToString());
            }


            int max = 0;

            for (int i = 0; i < projects.Rows.Count; i++)
            {
                int current = data.Tables[projects.Rows[i][1].ToString()].Rows.Count;
                if (current > max)
                    max = current;
            }


            DataTable show = new DataTable();

            for (int i = 0; i < max; i++)
                show.Rows.Add();

            for (int i = 0; i < projects.Rows.Count; i++)
            {
                string project_name = projects.Rows[i][1].ToString();

                show.Columns.Add(project_name);

                for (int j = 0; j < data.Tables[project_name].Rows.Count; j++)
                {
                     show.Rows[j][project_name] = data.Tables[project_name].Rows[j]["name"].ToString();
                }
            }
                
            dataGridView1.DataSource = show;


            //
            DBsql.close();
        }

        

    }
}