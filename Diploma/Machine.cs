using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma
{
    class Machine
    {
        //определение занятости аппарата
        bool busy;
        //
        string name;
        int id;
        


        public Machine(int id)
        {
            this.busy = false;
            switch (id)
            {
                case 0:
                    this.name = "Анализатор электролитов";
                    break;
                case 1:
                    this.name = "Автоматический анализатор глюкозы";
                    break;
                case 2:
                    this.name = "BS 3000";
                    break;
                case 3:
                    this.name = "ФЭК";
                    break;
                default:
                    break;
            }
            this.id = id;
        }


        public string Name
        {
            get
            {
                return name;
            }
        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        public bool Busy
        {
            get
            {
                return busy;
            }
        }
    }
}
