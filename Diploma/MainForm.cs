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
            }
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
