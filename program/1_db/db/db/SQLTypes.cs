using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    // наиболее употребимые типы
    enum SQLTypes
    {
        BIT,        // boolean. либо 0 либо 1
        TINYINT,    // от -128 до 127
        INT,        // от -2 147 483 648 до 2 147 483 647
        NVARCHAR,   // всегда после содержит круглые скобки с количеством символов в строке. NVARCHAR (300)
        DATETIME    // дата и время в формате ГГГГ-ММ-ДД ЧЧ:ММ:СС
    }
}
