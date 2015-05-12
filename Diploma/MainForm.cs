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
                                                              new KeyValuePair<string, byte>("ПТИ", 1),
                                                              new KeyValuePair<string, byte>("К,NA", 2),
                                                              new KeyValuePair<string, byte>("СРБ ,АСо", 3),
                                                              new KeyValuePair<string, byte>("Глюкоза", 4),
                                                              new KeyValuePair<string, byte>("Холестерин", 5),
                                                              new KeyValuePair<string, byte>("Бетта-липопротеиды,триглицириды", 6),
                                                              new KeyValuePair<string, byte>("ЛПНП,ЛПВП", 7),
                                                              new KeyValuePair<string, byte>("Билирубин общий", 8),
                                                              new KeyValuePair<string, byte>("Билирубин прямой", 9),
                                                              new KeyValuePair<string, byte>("АЛТ, АСТ", 10),
                                                              new KeyValuePair<string, byte>("Тимоловая проба", 11),
                                                              new KeyValuePair<string, byte>("Мочевина, Креатинин", 12),
                                                              new KeyValuePair<string, byte>("Общий белок", 13),
                                                              new KeyValuePair<string, byte>("Fe, ЖСС", 14),
                                                              new KeyValuePair<string, byte>("Альфа амил", 15),
                                                              new KeyValuePair<string, byte>("Мочевая кислота", 16),
                                                              new KeyValuePair<string, byte>("Щелочной фосфотазы", 17),
                                                              new KeyValuePair<string, byte>("ГГТП", 18)
                                                          };
            
           //создаём список диапазонов и вероятностей
           List<KeyValuePair<string, double>> ProbElements = new List<KeyValuePair<string, double>>
                                                         {
                                                             new KeyValuePair<string, double>("20,2-40,4 или 80,8-101 или 181,8-202", 0.014),
                                                             new KeyValuePair<string, double>("60,6-80,8", 0.043),
                                                             new KeyValuePair<string, double>("161,6-181,8", 0.057),
                                                             new KeyValuePair<string, double>("141,4-161,6", 0.100),
                                                             new KeyValuePair<string, double>("101-121,2", 0.157),
                                                             new KeyValuePair<string, double>("121,2-141,4", 0.186),
                                                             new KeyValuePair<string, double>("0-20.2", 0.414)
                                                         };

            double[][] ProbArray = new double[18][];
            ProbArray[0] = new double[7] {0.014,0.043,0.057,0.100,0.157,0.186,0.414};
            ProbArray[1] = new double[7] {0.014, 0.043, 0.057, 0.100, 0.157, 0.186, 0.414};
            ProbArray[2] = new double[9];
            ProbArray[3] = new double[7];
            
            Issled PTI = new Issled(IssledElements[0].Value, IssledElements[0].Key, ProbArray);
            
            PrintRichTxt(PTI.Id.ToString());
            PrintRichTxt(PTI.Name);
            for (int i = 0; i < 7; i++)
            {
                PrintRichTxt(PTI.Probability[0][i].ToString());
            }
                /*
                //генератор исследований на день для каждой пробы
                //инициализация генератора случайных чисел
                Random rndProbe = new Random();
                //объявляем переменную для хранения числа проб и генерируем её значение
                int diceRollProbe = rndProbe.Next(50, 536);
                //выводим число проб в поле на форме, переводя его в текстовый вид
                probeBox1.Text = diceRollProbe.ToString();
                //создаём 2D массив для определения исследований на каждую пробу
                int[,] issledArr = new int[18,diceRollProbe];
                //инициализация генератора случайных чисел
                Random rnd = new Random();
                //запускаем цикл для заполнения 2D массива
                for (int j = 0; j < 18; j++)
                {
                    for (int i = 0; i < diceRollProbe; i++)
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
                PrintRichTxt(matrixString);

                //создаём массив с названиями текстовых форм для вывода количества каждого исследования
                TextBox[] tbs = new TextBox[] { issledBox1, issledBox2, issledBox3, issledBox4, issledBox5, issledBox6, issledBox7, issledBox8, issledBox9, issledBox10, issledBox11,issledBox12,issledBox13,issledBox14,issledBox15,issledBox16,issledBox17,issledBox18 };
                //цикл вывода числа каждого исследования из массива в соответствующую текстовую форму
                for (int i = 0; i < tbs.Length; i++)
                {
                    tbs[i].Text = a[i].ToString();
                }*/
                //меняем текст на кнопке
                button1.Text = "Reset";
        }

        //очистка результатов на форме
        private void ClProg()
        {
            richTextBox1.Text = "";
            probeBox1.Text = "";
            button1.Text = "Start";          
        }

        //функция для печати в текстовой форме richTextBox1 с переносом строки
        private void PrintRichTxt(string s)
        {
            richTextBox1.Text += "\n" + s;
        }

        

        //переменная для определения статуса программы
        public bool progRun;
    }
}
