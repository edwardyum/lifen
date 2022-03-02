using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace app_db
{
    public static class Tools
    {
        public static string get_local_folder()
        {
            return ApplicationData.Current.LocalFolder.Path;
        }


    }
}
