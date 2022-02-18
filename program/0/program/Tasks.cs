using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace program
{
    class Tasks
    {
        // организация
        public int id;

        public int project_it;

        // отметка - следует ли вносить изменения в Excel. может это займёт много времени - перезаписывать все задачи всех проектов.
        public bool was_changed = false;

        public TaskStatus status = TaskStatus.In_work;

        // существо задачи
        public string name = "";
        public string description = "";

    }
}
