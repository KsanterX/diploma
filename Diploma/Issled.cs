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
        private double[][] probarray = new double[18][]
        {
            //массив значений для подсчета кумулятивной вероятности
                new double[7] { 0.014, 0.043, 0.057, 0.100, 0.157, 0.186, 0.414 },//ПТИ
                new double[7] { 0.014, 0.043, 0.057, 0.100, 0.214, 0.229, 0.271 },//К, Na
                new double[9] { 0.014, 0.043, 0.057, 0.071, 0.114, 0.143, 0.157, 0.186, 0.200 },//СРБ,АСО
                new double[7] { 0.014, 0.029, 0.057, 0.086, 0.143, 0.200, 0.243 },//Глюкоза
                new double[8] { 0.014, 0.029, 0.043, 0.129, 0.143, 0.157, 0.200, 0.271 },//Холестерин
                new double[7] { 0.014, 0.057, 0.071, 0.100, 0.129, 0.143, 0.200 },//Тимоловая
                new double[5] { 0.029, 0.057, 0.114, 0.157, 0.200 },//Мочев,креат
                new double[8] { 0.014, 0.029, 0.043, 0.057, 0.071, 0.143, 0.157, 0.171 },//Общ бел
                new double[7] { 0.014, 0.029, 0.071, 0.100, 0.114, 0.157, 0.229 },//Fe,жсс
                //
                new double[7] { 0.014, 0.029, 0.057, 0.086, 0.143, 0.200, 0.243 },//
                new double[8] { 0.014, 0.029, 0.043, 0.129, 0.143, 0.157, 0.200, 0.271 },
                new double[7] { 0.014, 0.043, 0.057, 0.100, 0.157, 0.186, 0.414 },
                new double[7] { 0.014, 0.043, 0.057, 0.100, 0.157, 0.186, 0.414 },
                new double[7] { 0.014, 0.043, 0.057, 0.100, 0.214, 0.229, 0.271 },
                new double[9] { 0.014, 0.043, 0.057, 0.071, 0.114, 0.143, 0.157, 0.186, 0.200 },
                new double[7] { 0.014, 0.029, 0.057, 0.086, 0.143, 0.200, 0.243 },
                new double[8] { 0.014, 0.029, 0.043, 0.129, 0.143, 0.157, 0.200, 0.271 },
                new double[7] { 0.014, 0.043, 0.057, 0.100, 0.157, 0.186, 0.414 },
        };
        //массив диапазонов
        int[] diap = new int[11] { 0, 20, 40, 61, 81, 101, 121, 141, 162, 182, 202 };


        public Issled(byte id, string name)
        {
            this.name = name;
            this.id = id;
            

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

        public double[][] ProbArray
        {
                
            
            get
            {
                return probarray;
            }
        }

        public int[] Diap
        {
            get
            {
                return diap;
            }
        }
        
    }
}
