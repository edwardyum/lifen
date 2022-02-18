using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_link_properties
{
    internal class MyTask
    {
        int row = 0;
        int col = 1;

        private string name = "";
        public string Name {
            get { return name; }
            set
            {
                name = value;
                Manager.table.Rows[row][col] = value;
            } }

        public MyTask()
        {
            name = "проба";
        }
    }
}
