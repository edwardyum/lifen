using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{
    internal static class SectionPages
    {
        // tags

        static public readonly string BlankPage1 = "BlankPage1";


        public static Type choose_page(string tag)
        {
            //// костыль для страницы настроек
            if (tag == "SettingsPage") tag = BlankPage1;
            ////

            Type type = Assembly.GetExecutingAssembly().GetType($"lifen.{tag}");
            return type;
        }
    }
}
