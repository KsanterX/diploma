using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma
{

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();


        }

        //при нажатии на кнопку, вызов функции проверки статуса запуска программы
        private void button1_Click(object sender, EventArgs e)
        {
            ProgCheck(progRun);
            
        }
        //проверка статуса программы.
        public void ProgCheck(bool pR)
        {
            //если программа не запускалась или была произведена перезагрузка, выполняется основная функция, иначе - очищается форма и ожидается запуск программы
            if (pR == false)
            {
                MainProg();
                progRun = true;
            }
            else
            {
                ClProg();
                progRun = false;
            }
        }

        //главная программа
        public void MainProg()
        {
            //список названий исследований и их id
            List<KeyValuePair<string, byte>> IssledElements = new List<KeyValuePair<string, byte>>
                                                          {
                                                              new KeyValuePair<string, byte>("ПТИ", 0),
                                                              new KeyValuePair<string, byte>("К,NA", 1),
                                                              new KeyValuePair<string, byte>("СРБ ,АСо", 2),
                                                              new KeyValuePair<string, byte>("Глюкоза", 3),
                                                              new KeyValuePair<string, byte>("Холестерин", 4),
                                                              new KeyValuePair<string, byte>("Бетта-липопротеиды,триглицириды", 5),
                                                              new KeyValuePair<string, byte>("ЛПНП,ЛПВП", 6),
                                                              new KeyValuePair<string, byte>("Билирубин общий", 7),
                                                              new KeyValuePair<string, byte>("Билирубин прямой", 8),
                                                              new KeyValuePair<string, byte>("АЛТ, АСТ", 9),
                                                              new KeyValuePair<string, byte>("Тимоловая проба", 10),
                                                              new KeyValuePair<string, byte>("Мочевина, Креатинин", 11),
                                                              new KeyValuePair<string, byte>("Общий белок", 12),
                                                              new KeyValuePair<string, byte>("Fe, ЖСС", 13),
                                                              new KeyValuePair<string, byte>("Альфа амил", 14),
                                                              new KeyValuePair<string, byte>("Мочевая кислота", 15),
                                                              new KeyValuePair<string, byte>("Щелочной фосфотазы", 16),
                                                              new KeyValuePair<string, byte>("ГГТП", 17)
            
                                                          };
            
            //массив объектов
            IDictionary<string, Issled> issl = new Dictionary<string, Issled>();
            for (int i = 0; i < IssledElements.Count; i++)
            {
                issl[IssledElements[i].Key] = new Issled(IssledElements[i].Value, IssledElements[i].Key);
            }

            //создаём массив с названиями текстовых форм для вывода количества каждого исследования
            TextBox[] tbs = new TextBox[] { issledBox1, issledBox2, issledBox3, issledBox4, issledBox5, issledBox6, issledBox7, issledBox8, issledBox9, issledBox10, issledBox11, issledBox12, issledBox13, issledBox14, issledBox15, issledBox16, issledBox17, issledBox18 };

            //выводим число проб в поле на форме, переводя его в текстовый вид
            int probe = ProbeGen();
            probeBox1.Text = probe.ToString();
            
            //переменная для подсчета кумулятивной вероятности
            double cumulative = 0.0;
            int ind = 0;
            int z = 0;
            int c = 0;

            //генератор
            Random r = new Random();
            double diceRoll;

            //цикл подсчета кумулятивной вероятности и сравнения с вероятностью из списка
            foreach (var x in issl.Values)
            {
                diceRoll = r.NextDouble();
                cumulative = 0;
                z = 0;
                c = 0;

                for (int i = 0; i < x.ProbArray[x.Id].GetLength(0); i++)
                {
                    //кумулятивная вероятность = кумулятивная вероятность + вероятность элемента списка
                    cumulative += x.ProbArray[x.Id][i,0];
                    //если сгенерированная вероятность меньше кумулятивной вероятности элемента списка
                    if (diceRoll < cumulative)
                    {
                        //если случаев вероятности больше 1
                        if (x.ProbArray[x.Id][i, 1] != 1)
                        {
                            //генерируем конкретный номер случая
                            z = r.Next(0,(int)x.ProbArray[x.Id][i,1]);
                                //определяем начальный индекс таблицы диапазонов
                                for (int j = 0; j < i; j++)
                                {
                                    c += (int)x.ProbArray[x.Id][j, 1];
                                }
                            //генерируем число анализов    
                            ind = r.Next(x.Diap[x.Id][c + z, 0], x.Diap[x.Id][c + z, 1]);
                        }
                        //если есть только один случай вероятности
                        else
                        {
                            //определяем индекс элемента для массива диапазонов
                            for (int j = 0; j < i; j++)
                            {
                                c += (int)x.ProbArray[x.Id][j, 1];
                            }
                            //генерируем число анализов
                            ind = r.Next(x.Diap[x.Id][c, 0], x.Diap[x.Id][c, 1]);
                        }
                        
                        //выводим в поле формы
                        if (ind > probe)
                        {
                            while (ind > probe)
                            {
                                ind--;
                            }
                        }
                        tbs[x.Id].Text = ind.ToString();                      
                        
                        break;
                    }
                }              
            }

                //меняем текст на кнопке
                button1.Text = "Reset";
        }

        //очистка результатов на форме
        private void ClProg()
        {
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            probeBox1.Text = "";
            button1.Text = "Start";          
        }

        //функция для печати в текстовой форме richTextBox1 с переносом строки
        private void PrintRichTxt(string s)
        {
            richTextBox1.Text += "\n" + s;
        }

        private int ProbeGen()
        {
            //массив кумулятивной вероятности проб
            double[,] probeArr = new double[7, 2] { {0.027,2},{0.041,1}, {0.068,1}, {0.095,2}, {0.122,2}, {0.135,1}, {0.270,1} };
            int[,] probeDiap = new int[10, 2] { {391,440}, {488,537}, {439,489}, {50,100}, {99,149}, {293,343}, {148,197}, {245,294}, {342,392}, {196,246} };
            double cumulative = 0;
            int z = 0;
            int c = 0;
            int probeAmount = 0;
            //генератор исследований на день для каждой пробы
            //инициализация генератора случайных чисел
            Random rndProbe = new Random();
            //объявляем переменную для хранения числа проб и генерируем её значение
            double diceRoll = rndProbe.NextDouble();

            for (int i = 0; i < probeArr.GetLength(0); i++)
            {
                cumulative += probeArr[i, 0];
                if (diceRoll < cumulative)
                {
                    if (probeArr[i, 1] != 1)
                    {
                        z = rndProbe.Next(0, (int)probeArr[i, 1]);
                        
                        for (int j = 0; j < i; j++)
                        {
                            c += (int)probeArr[j, 1];
                        }

                        probeAmount = rndProbe.Next(probeDiap[c + z, 0], probeDiap[c + z, 1]);
                    }
                    else
                    {
                        for (int j = 0; j < i; j++)
                        {
                            c += (int)probeArr[j, 1];
                        }

                        probeAmount = rndProbe.Next(probeDiap[c + z, 0], probeDiap[c + z, 1]);
                    }
                    break;
                }
            }
            return probeAmount;
        }

        public void IssledGen()
        {

        }

        //переменная для определения статуса программы
        public bool progRun;
    }
}
