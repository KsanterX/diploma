using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma
{
    class Issled
    {
        //имя исследования
        string name;
        //id исследования
        byte id;
        //массив вероятностей исследования
        double[][] probability = new double[18][];

        public Issled(byte id, string name, double[][] probability)
        {
            this.name = name;
            this.id = id;
            this.probability = probability;            
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public byte Id
        {
            get { return id; }
            set { id = value; }
        }

        public double[][] Probability
        {
            get { return probability; }
            set
            { 
                probability = value;
            }
        }
    }
}
