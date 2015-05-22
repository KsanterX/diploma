﻿using System;
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
        private double[][,] probarray = new double[13][,]
        {
            //массив значений для подсчета кумулятивной вероятности
                
                new double[6,2] { {0.014,3}, {0.043,1}, {0.100,2}, {0.214,1}, {0.229,1}, {0.271,1} },//К, Na
                
                new double[7,2] { {0.014,3}, {0.029,1}, {0.057,1}, {0.086,1}, {0.143,1}, {0.200,2}, {0.243,1} },//Глюкоза
                new double[8,2] { {0.014,2}, {0.029,1}, {0.043,1}, {0.129,1}, {0.143,1}, {0.157,1}, {0.200,1}, {0.271,1} },//Холестерин
                new double[8,2] { {0.014,1}, {0.029,1}, {0.057,2}, {0.071,1}, {0.114,2}, {0.143,1}, {0.157,1}, {0.243,1} },//Бетта-липопротеиды,триглицириды
                
                new double[7,2] { {0.029,3}, {0.043,1}, {0.071,1}, {0.114,1}, {0.143,1}, {0.157,2}, {0.229,1} },//Билирубин общий
                
                new double[8,2] { {0.029,3}, {0.043,1}, {0.086,1}, {0.129,1}, {0.143,1}, {0.157,1}, {0.171,1}, {0.186,1}},//АЛТ, АСТ
                new double[7,2] { {0.014,2}, {0.057,1}, {0.071,1}, {0.100,1}, {0.129,2}, {0.143,2}, {0.200,1} },//Тимоловая
                new double[5,2] { {0.029,2}, {0.057,2}, {0.114,1}, {0.157,2}, {0.200,2} },//Мочев,креат
                new double[8,2] { {0.014,1}, {0.029,1}, {0.043,1}, {0.057,1}, {0.071,1}, {0.143,1}, {0.157,3}, {0.171,1} },//Общ бел
                
                new double[7,2] { {0.014,2}, {0.029,1}, {0.071,1}, {0.157,1}, {0.186,1}, {0.229,1}, {0.300,1} },//Альфа амил
                new double[5,2] { {0.014,2}, {0.029,1}, {0.043,1}, {0.114,1}, {0.786,1} },//Мочевая кислота
                new double[8,2] { {0.014,2}, {0.043,1}, {0.057,1}, {0.086,1}, {0.100,2}, {0.143,1}, {0.171,1}, {0.271,1} },//Щелочной фосфотазы
                new double[5,2] { {0.029,1}, {0.043,1}, {0.100,1}, {0.343,1}, {0.486,1} },//ГГТП
        };
        //массив диапазонов
        int[][,] diap = new int[13][,]
        {

            new int[9,2] {{29,35}, {34,40}, {44,49}, {24,30}, {15,21}, {20,25}, {10,16}, {5,11}, {0,6}},//К, Na

            new int[10,2] {{250,286}, {285,322}, {321,357}, {0,37}, {72,108}, {214,251}, {36,73}, {107,144}, {178,215}, {143,179}},//Глюкоза
            new int[9,2] {{208,241}, {240,273}, {304,337}, {176,209}, {112,145}, {15,49}, {48,81}, {144,177}, {80,113}},//Холестерин
            new int[10,2] {{28,43}, {42,57}, {56,70}, {125,139}, {14,29}, {0,15}, {83,98}, {111,126}, {97,112}, {69,84}},//Бетта-липопротеиды,триглицириды

            new int[10,2] {{18,36}, {138,156}, {155,173}, {35,53}, {121,139}, {52,70}, {104,122}, {69,87}, {86,105}, {0,19}},//Билирубин общий

            new int[10,2] {{18,36}, {138,156}, {155,173}, {35,53}, {121,139}, {0,19}, {52,70}, {86,105}, {104,122}, {69,87}},//АЛТ, АСТ
            new int[10,2] {{33,50}, {147,164}, {131,148}, {17,34}, {115,132}, {0,18}, {66,83}, {49,67}, {82,99}, {98,116}},//Тимоловая
            new int[9,2] {{21,42}, {182,203}, {41,62}, {142,163}, {122,143}, {0,22}, {61,82}, {81,102}, {101,123}},//Мочев,креат
            new int[10,2] {{118,134}, {15,31}, {30,46}, {103,119}, {133,148}, {0,16}, {45,60}, {59,75}, {74,90}, {90,104}},//Общ бел
            
            new int[8,2] {{43,49}, {48,54}, {27,33}, {22,28}, {6,12}, {0,7}, {16,23}, {11,17}},//Альфа амил
            new int[6,2] {{38,44}, {43,48}, {10,16}, {15,20}, {5,11}, {0,5}},//Мочевая кислота
            new int[10,2] {{18,21}, {20,23}, {16,19}, {7,10}, {14,17}, {3,6}, {5,8}, {11,15}, {0,4}, {9,12}},//Щелочной фосфотазы
            new int[5,2] {{4,6}, {3,5}, {2,4}, {1,3}, {0,2}},//ГГТП

        };
        
        //массив затрат времени на исследования (в секундах)
        int[] timereq = new int[13] {30,120,180,1650,210,120,1950,120,1275,1920,795,60,135};

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

        public double[][,] ProbArray
        {
                
            
            get
            {
                return probarray;
            }
        }

        public int[][,] Diap
        {
            get
            {
                return diap;
            }
        }

        public int[] TimeReq
        {
            get
            {
                return timereq;
            }
        }
    }
}
