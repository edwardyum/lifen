using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{
    public static class Time
    {
        public static string now()
        {
            return DateTime.Now.ToString("G");
        }

        public static string now_date()
        {
            return DateTime.Now.ToString("d");
        }
    }
}
