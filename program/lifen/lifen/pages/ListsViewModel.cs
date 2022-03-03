using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifen
{
    internal class ListsViewModel
    {
        public Objective root { get; set; }

        public ListsViewModel()
        {
            root = Manager.root;
        }

    }
}
