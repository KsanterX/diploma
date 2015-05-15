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

            //PrintRichTxt(issl[IssledElements[0].Key].ProbArray[0].Length.ToString());
            
            //выводим на экран
            foreach (var x in issl.Values)
            {
                richTextBox1.Text += "\n" + "ID: " + x.Id.ToString() + " || " + "Имя: " + x.Name.ToString();
                   /* for (int i = 0; i < x.ProbArray[x.Id].GetLength(0); i++)
                    {
                        PrintRichTxt(x.ProbArray[x.Id][i,0].ToString());
                    }*/
            }


            //создаём массив с названиями текстовых форм для вывода количества каждого исследования
            TextBox[] tbs = new TextBox[] { issledBox1, issledBox2, issledBox3, issledBox4, issledBox5, issledBox6, issledBox7, issledBox8, issledBox9, issledBox10, issledBox11, issledBox12, issledBox13, issledBox14, issledBox15, issledBox16, issledBox17, issledBox18 };

            //выводим число проб в поле на форме, переводя его в текстовый вид
            int probe = ProbeGen(50, 536);
            probeBox1.Text = probe.ToString();
            
            //переменная для подсчета кумулятивной вероятности
            double cumulative = 0.0;
            int ind = 0;
            int z;
            string selectedElement;

            //генератор
            Random r = new Random();
            double diceRoll;

            //цикл подсчета кумулятивной вероятности и сравнения с вероятностью из списка
            foreach (var x in issl.Values)
            {
                diceRoll = r.NextDouble();
                cumulative = 0;

                for (int i = 0; i < x.ProbArray[x.Id].GetLength(0); i++)
                {
                    //кумулятивная вероятность = кумулятивная вероятность + вероятность элемента списка
                    cumulative += x.ProbArray[x.Id][i];
                    //если сгенерированная вероятность меньше кумулятивной вероятности элемента списка
                    if (diceRoll < cumulative)
                    {

                        //выводим отладочную информацию                        
                        selectedElement = "\n" + x.Id + " || " + x.Name;
                        richTextBox2.Text += selectedElement;
                        //генерируем число из диапазона
                        if (x.ProbAmount[x.Id][i] > 1)
                        {
                            z = r.Next(1,x.ProbAmount[x.Id][i]+1);
                            switch (x.Id)
                            {
                                case 0:
                                    switch (z)
                                    {
                                        case 1:
                                            ind = r.Next(x.Diap[x.Id][1], x.Diap[x.Id][2]);
                                            break;
                                        case 2:
                                            ind = r.Next(x.ProbAmount[x.Id][4], x.ProbAmount[x.Id][5]);
                                            break;
                                        case 3:
                                            ind = r.Next(x.ProbAmount[x.Id][9], x.ProbAmount[x.Id][10]);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case 1:
                                    switch (z)
                                    {
                                        case 1:
                                            ind = r.Next(x.ProbAmount[x.Id][6], x.ProbAmount[x.Id][7]);
                                            break;
                                        case 2:
                                            ind = r.Next(x.ProbAmount[x.Id][7], x.ProbAmount[x.Id][8]);
                                            break;
                                        case 3:
                                            ind = r.Next(x.ProbAmount[x.Id][10], x.ProbAmount[x.Id][11]);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case 2:
                                    switch (z)
                                    {
                                        case 1:
                                            ind = r.Next(x.ProbAmount[x.Id][9], x.ProbAmount[x.Id][10]);
                                            break;
                                        case 2:
                                            ind = r.Next(x.ProbAmount[x.Id][10], x.ProbAmount[x.Id][11]);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case 3:
                                    switch (z)
                                    {
                                        case 1:
                                            ind = r.Next(x.ProbAmount[x.Id][1], x.ProbAmount[x.Id][2]);
                                            break;
                                        case 2:
                                            ind = r.Next(x.ProbAmount[x.Id][4], x.ProbAmount[x.Id][5]);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case 4:
                                    switch (z)
                                    {
                                        case 1:
                                            ind = r.Next(x.ProbAmount[x.Id][1], x.ProbAmount[x.Id][2]);
                                            break;
                                        case 2:
                                            ind = r.Next(x.ProbAmount[x.Id][4], x.ProbAmount[x.Id][5]);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                default:
                                    ind = 5959;
                                    break;
                                     
                            }
                            
                        }
                        else if (i>0)
                        {
                            ind = r.Next(x.Diap[x.Id][i-1], x.Diap[x.Id][i]);
                        }
                        else
                        {
                            ind = r.Next(x.Diap[x.Id][i], x.Diap[x.Id][i + 1]);
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
                    else
                    {
                        continue;
                    }
                }              
            }

            
            
            
           /*
                //если сгенерированная вероятность меньше кумулятивной вероятности элемента списка
                if (diceRoll < cumulative)
                {
                    //добавил дополнительную проверку, поскольку три элемента в списке имеют одинаковую вероятность
                    if (elements[i].Value == 0.014)
                    {
                        //генерируем новое число от 1 до 3 и выбираем какому элементу присвоить выпадение
                        Random a = new Random(DateTime.Now.Millisecond);
                        int ind = r.Next(1, 3);
                        switch (ind)
                        {
                            case 1:
                                selectedElement = elements[i].Key;
                                PrintRichTxt(selectedElement);
                                PrintRichTxt("Точнее 20,2-40,4");
                                button1.Text = "Reset!";
                                break;
                            case 2:
                                selectedElement = elements[i].Key;
                                PrintRichTxt(selectedElement);
                                PrintRichTxt("Точнее 80,8-101");
                                button1.Text = "Reset!";
                                break;
                            default:
                                selectedElement = elements[i].Key;
                                PrintRichTxt(selectedElement);
                                PrintRichTxt("Точнее 181,8-202");
                                button1.Text = "Reset!";
                                break;
                        }
                    }
                    else
                    {
                        //иначе указываем диапазон, в который попадает сгенерированное число
                        selectedElement = elements[i].Key;
                        PrintRichTxt(selectedElement);
                        button1.Text = "Reset!";
                        break;
                    }
                }
            }*/
            
            
            /*//создаём 2D массив для определения исследований на каждую пробу
                int[,] issledArr = new int[18,probe];
                //инициализация генератора случайных чисел
                Random rnd = new Random();
                //запускаем цикл для заполнения 2D массива
                for (int j = 0; j < 18; j++)
                {
                    for (int i = 0; i < probe; i++)
                    {
                        //заполняем массив случайным образом числами 0 и 1
                        issledArr[j, i] = rnd.Next(0,2);
                    }
                }

                //создаём массив для хранения общего числа конкретных 18-ти исследований
                int[] a = new int[18];
                //вывод 2D массива в текстовое поле для проверки 
                string matrixString = "";
                for (int i = 0; i < issledArr.GetLength(0); i++)
                {
                    for (int j = 0; j < issledArr.GetLength(1); j++)
                    {
                        matrixString += issledArr[i, j].ToString();
                        matrixString += " ";
                        //складываем все числа по строкам и сохраняем общее число для каждого исследования
                        a[i] += issledArr[i, j];
                    }

                    matrixString += Environment.NewLine;
                }
                //вызываем функцию для печати строк из 2D массива в текстовом поле
               richTextBox2.Text = matrixString;*/

                
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

        private int ProbeGen(int min, int max)
        {
            //генератор исследований на день для каждой пробы
                //инициализация генератора случайных чисел
                Random rndProbe = new Random();
                //объявляем переменную для хранения числа проб и генерируем её значение
                int diceRollProbe = rndProbe.Next(min, max);
                return diceRollProbe;
        }

        public void IssledGen()
        {

        }

        //переменная для определения статуса программы
        public bool progRun;
    }
}
