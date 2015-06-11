using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Diploma
{
    

    public partial class MainForm : Form
    {
        #region FIELDS

        AlertForm alert;
        //переменная для определения статуса программы
        bool progRun;
        
        #endregion

        //создаём массив с названиями текстовых форм для вывода количества каждого исследования
        private static TextBox[] tbs;

        public MainForm()
        {
            InitializeComponent();

        }
        
        
        //при нажатии на кнопку, вызов функции проверки статуса запуска программы
        private void button1_Click(object sender, EventArgs e)
        {
          if (backgroundWorker1.IsBusy != true)
                {
                ProgCheck();
                }
            
        }
        //проверка статуса программы.
        public void ProgCheck()
        {
            //если программа не запускалась или была произведена перезагрузка, выполняется основная функция, иначе - очищается форма и ожидается запуск программы
            if (progRun == false)
            {
                //MainProg();
                    progRun = true;
                    alert = new AlertForm();
                    alert.Show();
                    
                    //Запуск фонового асинхронного процесса
                    backgroundWorker1.RunWorkerAsync();

            }
            else
            {
                ClProg();
                progRun = false;
            }
        }

        //static Random r = new Random((int)DateTime.Now.Ticks);
        static CryptoRandom r = new CryptoRandom();
        //главная программа



        public void MainProg()
        {

            //массив для хранения значений времени для каждого способа в течение одного цикла
            TimeSpan[] time;
            //массив общего времени для каждого дня и каждого способа
            TimeSpan[,] times;
            //переменная для расчета среднего значения времени
            TimeSpan average;
            //массив времени аппаратов
            TimeSpan[][] timeMachine = new TimeSpan[3][]
            {
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
            };
            //переменная для подсчета кумулятивной вероятности
            double cumulative = 0.0;
            //вспомогательные переменные
            int ind = 0;
            int z = 0;
            int c = 0;
            //количество проб
            int probe = 0;
            int probemax = 0;
            //количество дней
            int daybox;
            Probe pr = new Probe();
            //генератор
            //Random r = new Random((int)DateTime.Now.Ticks);
            double diceRoll;
            //список названий исследований и их id
            List<KeyValuePair<string, byte>> IssledElements = new List<KeyValuePair<string, byte>>
                                                          {
                                                              
                                                              new KeyValuePair<string, byte>("К,NA", 0),
                                                              
                                                              new KeyValuePair<string, byte>("Глюкоза", 1),
                                                              new KeyValuePair<string, byte>("Холестерин", 2),
                                                              new KeyValuePair<string, byte>("Бетта-липопротеиды,триглицириды", 3),
                                                              
                                                              new KeyValuePair<string, byte>("Билирубин общий", 4),
                                                              
                                                              new KeyValuePair<string, byte>("АЛТ, АСТ", 5),
                                                              new KeyValuePair<string, byte>("Тимоловая проба", 6),
                                                              new KeyValuePair<string, byte>("Мочевина, Креатинин", 7),
                                                              new KeyValuePair<string, byte>("Общий белок", 8),
                                                              
                                                              new KeyValuePair<string, byte>("Альфа амил", 9),
                                                              new KeyValuePair<string, byte>("Мочевая кислота", 10),
                                                              new KeyValuePair<string, byte>("Щелочной фосфотазы", 11),
                                                              new KeyValuePair<string, byte>("ГГТП", 12)
            
                                                          };
            
            //массив объектов
            IDictionary<string, Issled> issl = new Dictionary<string, Issled>();
            for (int i = 0; i < IssledElements.Count; i++)
            {
                issl[IssledElements[i].Key] = new Issled(IssledElements[i].Value, IssledElements[i].Key);
            }

             tbs = new TextBox[] { issledBox1, issledBox2, issledBox3, issledBox4, issledBox5, issledBox6, issledBox7, issledBox8, issledBox9, issledBox10, issledBox11, issledBox12, issledBox13 };

            //создаём массив аппаратов
            IDictionary<int, Machine> mach = new Dictionary<int, Machine>();
            for (int i = 0; i <= 3; i++)
            {
                mach[i] = new Machine(i);
            }

            //проверка количества дней
            if (dayBox.Text != "")
            {
                daybox = Convert.ToInt32(dayBox.Text);
            }
            else
            {
                daybox = 1;
                dayBox.Text = daybox.ToString();
            }

            //создание массивов для хранения значений времени
            times = new TimeSpan[daybox,3];
            time = new TimeSpan[3];

            //цикл по количеству дней
            for (int day = 1; day <= daybox; day++)
            {
                Invoke(new Action(() => richTextBox1.Text += "\n **********"));
                Invoke(new Action(() => richTextBox1.Text += "\n ДЕНЬ " + day));
                Invoke(new Action(() => richTextBox1.Text += "\n **********"));
                
                //выводим число проб в поле на форме, переводя его в текстовый вид
                probe = ProbeGen();
                Invoke(new Action(() => richTextBox1.Text += "\n Число проб: " + probe));
                probemax += probe;
                
                //обнуление переменных и массивов
                cumulative = 0.0;
                z = 0;
                c = 0;
                Array.Clear(time,0,time.Length);
                
                foreach (TimeSpan[] subArray in timeMachine)
                {
                    Array.Clear (subArray, 0, subArray.Length);
                }

                //массив проб             
                pr.probeCount = new int[probe, 13];

                //цикл подсчета кумулятивной вероятности и сравнения с вероятностью из списка
                foreach (var x in issl.Values)
                {
                    diceRoll = r.NextDouble();
                    cumulative = 0;
                    z = 0;
                    c = 0;
                    ind = 0;

                    for (int i = 0; i < x.ProbArray[x.Id].GetLength(0); i++)
                    {
                        //кумулятивная вероятность = кумулятивная вероятность + вероятность элемента списка
                        cumulative += x.ProbArray[x.Id][i, 0] * x.ProbArray[x.Id][i, 1];
                        //если сгенерированная вероятность меньше кумулятивной вероятности элемента списка
                        if (diceRoll < cumulative)
                        {
                            //если случаев вероятности больше 1
                            if (x.ProbArray[x.Id][i, 1] != 1)
                            {

                                //генерируем конкретный номер случая
                                z = r.Next(0, (int)x.ProbArray[x.Id][i, 1]);
                                //определяем начальный индекс таблицы диапазонов
                                if (i != 0)
                                {
                                    for (int j = 0; j < i; j++)
                                    {
                                        c += (int)x.ProbArray[x.Id][j, 1];
                                    }
                                }
                                else
                                {
                                    c = i;
                                }
                                //генерируем число анализов    
                                ind = r.Next(x.Diap[x.Id][c + z, 0], x.Diap[x.Id][c + z, 1]);
                            }
                            //если есть только один случай вероятности
                            else
                            {
                                //определяем индекс элемента для массива диапазонов
                                if (i != 0)
                                {
                                    for (int j = 0; j < i; j++)
                                    {
                                        c += (int)x.ProbArray[x.Id][j, 1];
                                    }
                                }
                                else
                                {
                                    c = i;
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
                            if (tbs[x.Id].Text != "")
                            {
                                z = Convert.ToInt32(tbs[x.Id].Text);
                                ind += z;
                            }                           
                            Invoke(new Action(() => tbs[x.Id].Text = ind.ToString()));
                            pr.probeC(probe, x.Id, ind);
                            break;
                        }
                    }
                }

                    //смотрим сколько раз будет запускаться центрифуга
                    z = probe / 20;
                    if (probe % 20 > 0)
                    {
                        z++;
                    }
                   
                    //3 способа
                    for (int sposob = 0; sposob < 3; sposob++)
                    {
                        switch (sposob)
                        {
#region Способ 1
                            case 0:
                                /*Thread s1 = new Thread(Sposob1);
                                s1.Start();*/
                                Invoke(new Action(() => richTextBox1.Text += "\n ----------"));
                                Invoke(new Action(() => richTextBox1.Text += "\n Способ 1: "));
                                Invoke(new Action(() => richTextBox1.Text += "\n ----------"));
                                //запускаем центрифугу
                                for (int i = 0; i < z; i++)
                                {
                                    time[sposob] += TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(r.Next(1, 31));
                                }


                                Invoke(new Action(() => richTextBox1.Text += "\n Время центрифугирования: " + time[sposob]));

                                foreach (var m in mach.Values)
                                {
                                    switch (m.Id)
                                    {
                                        case 0:
                                            for (int i = 0; i < pr.probeCount.GetLength(0); i++)
                                            {
                                                //К
                                                if (pr.probeCount[i, 0] != 0)
                                                {

                                                    timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(30);
                                                }
                                            }
                                            timeMachine[sposob][m.Id] = DayHours(timeMachine[sposob][m.Id]);
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            break;
                                        case 1:
                                            for (int i = 0; i < pr.probeCount.GetLength(0); i++)
                                            {
                                                //глюкоза(2)
                                                if (pr.probeCount[i, 1] != 0)
                                                {

                                                    timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                }
                                            }
                                            timeMachine[sposob][m.Id] = DayHours(timeMachine[sposob][m.Id]);
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            break;
                                        case 2:
                                            for (int i = 0; i < pr.probeCount.GetLength(0); i += 20)
                                            {
                                                //холестерин(3)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 2] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(32);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 2] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(32);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Бетта-липопротеиды,триглицириды (27.30)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 3] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(35);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 3] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(35);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Билирубин общий(3.30)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 4] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(20);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 4] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(20);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //АЛТ, АСТ(1)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 5] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(15);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 5] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(15);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Мочев,креат(1)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 7] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(20);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 7] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(20);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Общ бел (21)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 8] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds(5);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 8] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds(5);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Альфа амил (32)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 9] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 9] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Щелочной фосфотазы
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 11] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 11] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //ГГТП(2.15)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 12] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(25);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 12] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(25);
                                                            break;
                                                        }

                                                    }
                                                }
                                            }
                                            timeMachine[sposob][m.Id] = DayHours(timeMachine[sposob][m.Id]);
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            break;
                                        case 3:
                                            //по 20
                                            for (int i = 0; i < pr.probeCount.GetLength(0); i += 20)
                                            {
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 6] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(32) + TimeSpan.FromSeconds(30);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 6] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(32) + TimeSpan.FromSeconds(30);
                                                            break;
                                                        }

                                                    }
                                                }

                                                //

                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 10] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(13);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 10] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(13);
                                                            break;
                                                        }

                                                    }
                                                }
                                            }
                                            timeMachine[sposob][m.Id] = DayHours(timeMachine[sposob][m.Id]);
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                time[sposob] = timeMachine[sposob].Max() + TimeSpan.FromSeconds(r.Next(60, Convert.ToInt32(time[sposob].TotalSeconds)));

                                times[day - 1, sposob] = DayHours(time[sposob]);

                                Invoke(new Action(() => richTextBox1.Text += "\n Общее затраченное время: " + times[day-1,sposob]));

                                
                                break;
#endregion
#region Способ 2
                            case 1:

                                Invoke(new Action(() => richTextBox1.Text += "\n ----------"));
                                Invoke(new Action(() => richTextBox1.Text += "\n Способ 2: "));
                                Invoke(new Action(() => richTextBox1.Text += "\n ----------"));
                                //запускаем центрифугу
                                for (int i = 0; i < z; i++)
                                {
                                    time[sposob] += TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(r.Next(1, 31));
                                }


                                Invoke(new Action(() => richTextBox1.Text += "\n Время центрифугирования: " + time[sposob]));

                                foreach (var m in mach.Values)
                                {
                                    switch (m.Id)
                                    {
                                        case 0:
                                            for (int i = 0; i < pr.probeCount.GetLength(0); i++)
                                            {
                                                //К
                                                if (pr.probeCount[i, 0] != 0)
                                                {

                                                    timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(30) + TimeSpan.FromSeconds(r.Next(10,35));
                                                }
                                            }
                                            timeMachine[sposob][m.Id] = DayHours(timeMachine[sposob][m.Id]);
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            break;
                                        case 1:
                                            for (int i = 0; i < pr.probeCount.GetLength(0); i++)
                                            {
                                                //глюкоза(2)
                                                if (pr.probeCount[i, 1] != 0)
                                                {

                                                    timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                }
                                            }
                                            timeMachine[sposob][m.Id] = DayHours(timeMachine[sposob][m.Id]);
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            break;
                                        case 2:
                                            for (int i = 0; i < pr.probeCount.GetLength(0); i += 20)
                                            {
                                                //холестерин(3)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 2] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(32);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 2] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(32);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Бетта-липопротеиды,триглицириды (27.30)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 3] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(35);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 3] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(35);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Билирубин общий(3.30)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 4] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(20);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 4] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(20);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //АЛТ, АСТ(1)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 5] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(15);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 5] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(15);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Мочев,креат(1)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 7] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(20);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 7] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(20);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Общ бел (21)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 8] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds(5);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 8] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds(5);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Альфа амил (32)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 9] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 9] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Щелочной фосфотазы
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 11] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 11] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //ГГТП(2.15)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 12] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(25);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 12] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(25);
                                                            break;
                                                        }

                                                    }
                                                }
                                            }
                                            timeMachine[sposob][m.Id] = DayHours(timeMachine[sposob][m.Id]);
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            break;
                                        case 3:
                                            //по 20
                                            for (int i = 0; i < pr.probeCount.GetLength(0); i += 20)
                                            {
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 6] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(32) + TimeSpan.FromSeconds(30);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 6] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(32) + TimeSpan.FromSeconds(30);
                                                            break;
                                                        }

                                                    }
                                                }

                                                //

                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 10] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(13);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 10] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(13);
                                                            break;
                                                        }

                                                    }
                                                }
                                            }

                                            timeMachine[sposob][m.Id] = DayHours(timeMachine[sposob][m.Id]);
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                time[sposob] = timeMachine[sposob].Max() + TimeSpan.FromSeconds(r.Next(60, Convert.ToInt32(time[sposob].TotalSeconds)));
                                times[day - 1, sposob] = DayHours(time[sposob]);

                                Invoke(new Action(() => richTextBox1.Text += "\n Общее затраченное время: " + times[day - 1, sposob]));


                                break;
#endregion
#region Способ 3
                            case 2:
                                Invoke(new Action(() => richTextBox1.Text += "\n ----------"));
                                Invoke(new Action(() => richTextBox1.Text += "\n Способ 3: "));
                                Invoke(new Action(() => richTextBox1.Text += "\n ----------"));
                                //запускаем центрифугу
                                for (int i = 0; i < z; i++)
                                {
                                    time[sposob] += TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(r.Next(1, 31));
                                }


                                Invoke(new Action(() => richTextBox1.Text += "\n Время центрифугирования: " + time[sposob]));

                                foreach (var m in mach.Values)
                                {
                                    switch (m.Id)
                                    {
                                        case 0:
                                            for (int i = 0; i < pr.probeCount.GetLength(0); i++)
                                            {
                                                //К
                                                if (pr.probeCount[i, 0] != 0)
                                                {

                                                    timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(20);
                                                }
                                            }
                                            timeMachine[sposob][m.Id] = DayHours(timeMachine[sposob][m.Id]);
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            break;
                                        case 1:
                                            for (int i = 0; i < pr.probeCount.GetLength(0); i++)
                                            {
                                                //глюкоза(2)
                                                if (pr.probeCount[i, 1] != 0)
                                                {

                                                    timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                }
                                            }
                                            timeMachine[sposob][m.Id] = DayHours(timeMachine[sposob][m.Id]);
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            break;
                                        case 2:
                                            for (int i = 0; i < pr.probeCount.GetLength(0); i += 20)
                                            {
                                                //холестерин(3)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 2] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(32);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 2] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(32);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Бетта-липопротеиды,триглицириды (27.30)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 3] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(35);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 3] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(35);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Билирубин общий(3.30)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 4] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(20);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 4] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(20);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //АЛТ, АСТ(1)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 5] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(15);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 5] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(15);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Мочев,креат(1)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 7] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(20);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 7] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(20);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Общ бел (21)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 8] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds(5);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 8] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds(5);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Альфа амил (32)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 9] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 9] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //Щелочной фосфотазы
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 11] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 11] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(1);
                                                            break;
                                                        }

                                                    }
                                                }
                                                //ГГТП(2.15)
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 12] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(25);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 12] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromSeconds(25);
                                                            break;
                                                        }

                                                    }
                                                }
                                            }
                                            timeMachine[sposob][m.Id] = DayHours(timeMachine[sposob][m.Id]);
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            break;
                                        case 3:
                                            //по 20
                                            for (int i = 0; i < pr.probeCount.GetLength(0); i += 20)
                                            {
                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 6] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(32) + TimeSpan.FromSeconds(30);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 6] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(32) + TimeSpan.FromSeconds(30);
                                                            break;
                                                        }

                                                    }
                                                }

                                                //

                                                if (pr.probeCount.GetLength(0) - 20 >= i)
                                                {
                                                    for (int j = 0; j <= 20; j++)
                                                    {
                                                        if (pr.probeCount[j, 10] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(13);
                                                            break;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    for (int j = 0; j <= i - pr.probeCount.GetLength(0); j++)
                                                    {
                                                        if (pr.probeCount[j, 10] > 0)
                                                        {
                                                            timeMachine[sposob][m.Id] += TimeSpan.FromMinutes(13);
                                                            break;
                                                        }

                                                    }
                                                }
                                            }
                                            timeMachine[sposob][m.Id] = DayHours(timeMachine[sposob][m.Id]);
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                time[sposob] = timeMachine[sposob].Max() + TimeSpan.FromSeconds(r.Next(60, Convert.ToInt32(time[sposob].TotalSeconds)));
                                

                                times[day - 1, sposob] = DayHours(time[sposob]);

                                Invoke(new Action(() => richTextBox1.Text += "\n Общее затраченное время: " + times[day - 1, sposob]));


                                break;
#endregion
                            default:
                                break;
                        }
                        
                    }
                    backgroundWorker1.ReportProgress(100*day/daybox);
                }
                
                //Вывод общих результатов               
                Invoke(new Action(() => richTextBox2.Text += "\n Всего дней обработано: " + daybox));
                Invoke(new Action(() => richTextBox2.Text += "\n Всего проб взято: " + probemax));
                Invoke(new Action(() => richTextBox2.Text += "\n Среднее число проб в день: " + probemax / daybox));
                //Способ 1
                Invoke(new Action(() => richTextBox2.Text += "\n ------------------------------------------------"));
                Invoke(new Action(() => richTextBox2.Text += "\n Способ 1."));
                Invoke(new Action(() => richTextBox2.Text += "\n ------------------------------------------------"));
                //обнуляем значение среднего
                average = TimeSpan.Zero;
                //вычисляем средний результат
                for (int i = 0; i < times.GetLength(0);i++)
                {
                        average += TimeSpan.FromSeconds(times[i,0].TotalSeconds);                    
                }
                average = TimeSpan.FromSeconds(average.TotalSeconds / daybox);
                
                //публикуем значение среднего
                Invoke(new Action(() => richTextBox2.Text += "\n Среднее затраченное время: " + average));
                Invoke(new Action(() => richTextBox2.Text += "\n ------------------------------------------------"));
                //Способ 2
                Invoke(new Action(() => richTextBox2.Text += "\n ------------------------------------------------"));
                Invoke(new Action(() => richTextBox2.Text += "\n Способ 2."));
                Invoke(new Action(() => richTextBox2.Text += "\n ------------------------------------------------"));

                average = TimeSpan.Zero;
                for (int i = 0; i < times.GetLength(0); i++)
                {
                    average += TimeSpan.FromSeconds(times[i, 1].TotalSeconds);
                }
                average = TimeSpan.FromSeconds(average.TotalSeconds / daybox);

                Invoke(new Action(() => richTextBox2.Text += "\n Среднее затраченное время: " + average));
                Invoke(new Action(() => richTextBox2.Text += "\n ------------------------------------------------"));
                //Способ 3
                Invoke(new Action(() => richTextBox2.Text += "\n ------------------------------------------------"));
                Invoke(new Action(() => richTextBox2.Text += "\n Способ 3."));
                Invoke(new Action(() => richTextBox2.Text += "\n ------------------------------------------------"));

                average = TimeSpan.Zero;
                for (int i = 0; i < times.GetLength(0); i++)
                {
                    average += TimeSpan.FromMilliseconds(times[i,2].TotalMilliseconds);
                }
                average = TimeSpan.FromSeconds(average.TotalSeconds / daybox);

                Invoke(new Action(() => richTextBox2.Text += "\n Среднее затраченное время: " + average));
                Invoke(new Action(() => richTextBox2.Text += "\n ------------------------------------------------"));
                
                //меняем текст на кнопке
                Invoke(new Action(() => button1.Text = "Reset"));
        }

        //учёт времени
        private TimeSpan DayHours(TimeSpan seed)
        {

            if (seed.TotalMinutes >= 480)
            {
                seed = TimeSpan.FromMinutes(480) - TimeSpan.FromMinutes(r.Next(1, r.Next(2,25)));
            }

            return seed;
        }

        //очистка результатов на форме
        private void ClProg()
        {
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            //dayBox.Text = "1";
            button1.Text = "Start";
            for (int i = 0; i < tbs.Length; i++)
            {
                tbs[i].Text = "";
            }
        }

        //функция для печати в текстовой форме richTextBox1 с переносом строки
        private void PrintRichTxt(string s)
        {
            richTextBox1.Text += "\n" + s;
        }

        //функция генерации количества проб
        private int ProbeGen()
        {
            //массив подсчета кумулятивной вероятности проб
            double[,] probeArr = new double[7, 2] { {0.027,2},{0.041,1}, {0.068,1}, {0.095,2}, {0.122,2}, {0.135,1}, {0.270,1} };
            int[,] probeDiap = new int[10, 2] { {391,440}, {488,537}, {439,489}, {50,100}, {99,149}, {293,343}, {148,197}, {245,294}, {342,392}, {196,246} };
            double cumulative = 0.0;
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
                cumulative += probeArr[i, 0]*probeArr[i,1];
                if (diceRoll < cumulative)
                {
                    if (probeArr[i, 1] != 1)
                    {
                        z = rndProbe.Next(0, (int)probeArr[i, 1]);

                        if (i != 0)
                        {
                            for (int j = 0; j < i; j++)
                            {
                                c += (int)probeArr[j, 1];
                            }
                        }
                        else
                        {
                            c = i;
                        }
                        
                        probeAmount = rndProbe.Next(probeDiap[c + z, 0], probeDiap[c + z, 1]);
                    }
                    else
                    {
                        if (i != 0)
                        {
                            for (int j = 0; j < i; j++)
                            {
                                c += (int)probeArr[j, 1];
                            }
                        }
                        else
                        {
                            c = i;
                        }
                        
                        probeAmount = rndProbe.Next(probeDiap[c + z, 0], probeDiap[c + z, 1]);
                    }
                    break;
                }
            }
            return probeAmount;
        }

        //фоновый поток для основных вычислений
        void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
         BackgroundWorker worker = sender as BackgroundWorker;
         MainProg();
        }

        //обновление полосы прогресса
        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            alert.Message = "Выполняется симуляция, подождите... " + e.ProgressPercentage.ToString() + "%";
            alert.ProgressValue = e.ProgressPercentage;
        }

        //завершение фонового процесса
        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {          
            alert.Close();
        }

 
    }
}
