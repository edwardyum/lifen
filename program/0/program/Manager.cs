using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace program
{
    class Manager
    {
        All all = new All(); // содержит всю информацию
                             // все загруженные листы из базы данных

        Parser parser = new Parser();
        Calendar calendar = new Calendar();
        Analytics analytics = new Analytics();
    }
}
