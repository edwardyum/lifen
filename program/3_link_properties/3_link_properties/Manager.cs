using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_link_properties
{
    internal static class Manager
    {
        public static DataTable table = new DataTable();
        
        public static void fill()
        {
            table.Columns.Add();
            table.Columns.Add();

            table.Rows.Add();
            table.Rows.Add();

            table.Rows[0][0] = "1"; table.Rows[0][1] = "2";
            table.Rows[1][0] = "3"; table.Rows[1][1] = "4";
        }

    }
}
