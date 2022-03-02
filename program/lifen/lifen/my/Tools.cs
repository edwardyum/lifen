using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{

    // ДОБАВИТЬ КЛАСС В БИБЛИОТЕКУ

    internal static class Tools
    {
        public static string get_assembly_dirrectory()          // папка в которой находится исполняемый файл
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }
    }
}
