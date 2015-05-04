using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma
{
    class Issled
    {
        private string name;
        private int id;
        private int diap;

        public void Diap(int id)
        {
            switch (id)
            {
                case 1:
                    diap = 202;
                    break;
                default:
                    break;
            }
        }


        public Issled(int id, string name)
        {
            this.name = name;
            this.id = id;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
    }
}
