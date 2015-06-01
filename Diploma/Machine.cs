using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma
{
    //класс аппаратов
    class Machine : Actor
    {
        //определение занятости аппарата
        bool busy;
        public string Name { get; set; }

        public Machine(int id) : base (id)
        {
            this.busy = false;
            switch (id)
            {
                case 0:
                    Name = "Анализатор электролитов";
                    break;
                case 1:
                    Name = "Автоматический анализатор глюкозы";
                    break;
                case 2:
                    Name = "BS 3000";
                    break;
                case 3:
                    Name = "ФЭК";
                    break;
                default:
                    break;
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
