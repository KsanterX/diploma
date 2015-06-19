using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Diploma
{
    

    public partial class MainForm : Form
    {
        #region FIELDS

        AlertForm alert;
        AboutBox1 aboutbox = new AboutBox1();
        InfoForm iForm = new InfoForm();
        InputForm inputform = new InputForm();
        //переменная для определения статуса программы
        bool progRun;
        //количество дней
        public static int daybox;      
        
        
        #endregion

        //создаём массив с названиями текстовых форм для вывода количества каждого исследования
        private static TextBox[] tbs;

        public MainForm()
        {
            InitializeComponent();
            daybox = 1;
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
            TimeSpan[][] timeMachine = new TimeSpan[27][]
            {
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
            };
            //переменная для подсчета кумулятивной вероятности
            double cumulative = 0.0;
            //Реальное время
            TimeSpan realtime = TimeSpan.Zero;
            //вспомогательные переменные
            int ind = 0;
            int z = 0;
            int c = 0;
            //количество проб
            int probe = 0;
            int probemax = 0;
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

            //создание массивов для хранения значений времени
            times = new TimeSpan[daybox,27];
            time = new TimeSpan[27];

            using (ExcelPackage pack = new ExcelPackage())
            {
                pack.Workbook.Properties.Author = "Ксения Андрианова";
                pack.Workbook.Properties.Title = "Диплом";
                pack.Workbook.Properties.Company = "Ксения";

                pack.Workbook.Worksheets.Add("Отчёт");
                ExcelWorksheet ws = pack.Workbook.Worksheets[1]; // 1 is the position of the worksheet
                ws.Name = "Отчёт";

                int rowIndex = 1;
                int colIndex = 1;


                do
                {
                    // Set the background colours
                    var cell = ws.Cells[rowIndex, colIndex];
                    var fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.LightGreen);
                    cell.Style.WrapText = true;
                    cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    colIndex++;
                }
                while (colIndex != 9);

                // Set the cell values
                ws.Cells[1, 1].Value = "День";
                ws.Cells[1, 2].Value = "Способ";
                foreach (var em in mach.Values)
                {
                    ws.Cells[1, em.Id + 3].Value = em.Name;
                }
                ws.Cells[1, 7].Value = "Общее затраченное время";
                ws.Cells[1, 7].Style.WrapText = true;
                ws.Cells[1, 7].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[1, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, 7].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                ws.Cells[1, 8].Value = "Количество проб";
                ws.Cells[1, 8].Style.WrapText = true;
                ws.Cells[1, 8].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[1, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, 8].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);

                ws.Cells[1, 15].Value = "Всего дней обработано";
                ws.Cells[1, 15].Style.WrapText = true;
                ws.Cells[1, 15].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[1, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, 15].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);

                ws.Cells[1, 16].Value = "Всего проб взято";
                ws.Cells[1, 16].Style.WrapText = true;
                ws.Cells[1, 16].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[1, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, 16].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);

                ws.Cells[1, 17].Value = "Среднее число проб в день";
                ws.Cells[1, 17].Style.WrapText = true;
                ws.Cells[1, 17].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[1, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, 17].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);

                ws.Cells[1, 18].Value = "Время в реальности";
                ws.Cells[1, 18].Style.WrapText = true;
                ws.Cells[1, 18].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[1, 18].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, 18].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);

                ws.Cells[10, 15].Value = "Способ";
                ws.Cells[10, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[10, 15].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                ws.Cells[10, 16].Value = "Среднее затраченное время";
                ws.Cells[10, 16].Style.WrapText = true;
                ws.Cells[10, 16].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[10, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[10, 16].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                ws.Cells[39, 14].Value = "Макс";
                ws.Cells[39, 14].Style.WrapText = true;
                ws.Cells[39, 14].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[39, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[39, 14].Style.Fill.BackgroundColor.SetColor(Color.Coral);
                ws.Cells[39, 16].Style.Numberformat.Format = "hh:mm:ss";
                ws.Cells[39, 16].Formula = "MAX(P11:P37)";
                ws.Cells[39, 15].Formula = "INDEX(O11:O37,MATCH(P39,P11:P37,0))";

                ws.Cells[40, 14].Value = "Мин";
                ws.Cells[40, 14].Style.WrapText = true;
                ws.Cells[40, 14].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[40, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[40, 14].Style.Fill.BackgroundColor.SetColor(Color.Coral);
                ws.Cells[40, 16].Style.Numberformat.Format = "hh:mm:ss";
                ws.Cells[40, 16].Formula = "MIN(P11:P37)";
                ws.Cells[40, 15].Formula = "INDEX(O11:O37,MATCH(P40,P11:P37,0))";

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
  
                    //способы
                    for (int sposob = 0; sposob < 27; sposob++)
                    {
#region Способ

                                Invoke(new Action(() => richTextBox1.Text += "\n ----------"));
                                Invoke(new Action(() => richTextBox1.Text += "\n Способ " + (sposob+1) + ": "));
                                Invoke(new Action(() => richTextBox1.Text += "\n ----------"));
                                rowIndex++;
                                if (sposob == 0)
                                {
                                    ws.Cells[rowIndex, 1].Value = day;
                                    ws.Cells[rowIndex, 8].Value = probe;
                                }
                                ws.Cells[rowIndex, 2].Value = sposob+1;
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
                                            timeMachine[sposob][m.Id] = DayHours((timeMachine[sposob][m.Id] + TimeSpan.FromMinutes(r.Next(0,sposob))));
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            ws.Cells[rowIndex, m.Id + 3].Style.Numberformat.Format = "hh:mm:ss";
                                            ws.Cells[rowIndex, m.Id + 3].Value = timeMachine[sposob][m.Id];
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
                                            timeMachine[sposob][m.Id] = DayHours((timeMachine[sposob][m.Id] + TimeSpan.FromMinutes(r.Next(0, sposob+r.Next(0,r.Next(30))))));
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            ws.Cells[rowIndex, m.Id + 3].Style.Numberformat.Format = "hh:mm:ss";
                                            ws.Cells[rowIndex, m.Id + 3].Value = timeMachine[sposob][m.Id];
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
                                            timeMachine[sposob][m.Id] = DayHours((timeMachine[sposob][m.Id] - TimeSpan.FromMinutes(r.Next(0, sposob))));
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            ws.Cells[rowIndex, m.Id + 3].Style.Numberformat.Format = "hh:mm:ss";
                                            ws.Cells[rowIndex, m.Id + 3].Value = timeMachine[sposob][m.Id];
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
                                            timeMachine[sposob][m.Id] = DayHours((timeMachine[sposob][m.Id] - TimeSpan.FromMinutes(r.Next(0, r.Next(0,sposob)))))-TimeSpan.FromMinutes(r.Next(10,25));
                                            Invoke(new Action(() => richTextBox1.Text += "\n" + m.Name + ": " + timeMachine[sposob][m.Id]));
                                            ws.Cells[rowIndex, m.Id + 3].Style.Numberformat.Format = "hh:mm:ss";
                                            ws.Cells[rowIndex, m.Id + 3].Value = timeMachine[sposob][m.Id];
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                time[sposob] = timeMachine[sposob].Max() + TimeSpan.FromSeconds(r.Next(60, Convert.ToInt32(time[sposob].TotalSeconds)));
                                
                                realtime = time[sposob] + TimeSpan.FromMinutes(r.Next(10,40));

                                times[day - 1, sposob] = DayHours(time[sposob]);

                                Invoke(new Action(() => richTextBox1.Text += "\n Общее затраченное время: " + times[day-1,sposob]));
                                ws.Cells[rowIndex, 7].Style.Numberformat.Format = "hh:mm:ss";
                                ws.Cells[rowIndex, 7].Value = times[day - 1, sposob];

#endregion
                        
                    }
                    backgroundWorker1.ReportProgress(100*day/daybox);
            }

                //Вывод общих результатов               
                Invoke(new Action(() => richTextBox2.Text += "\n Всего дней обработано: " + daybox));
                ws.Cells[2, 15].Value = daybox;
                Invoke(new Action(() => richTextBox2.Text += "\n Всего проб взято: " + probemax));
                ws.Cells[2, 16].Value = probemax;
                Invoke(new Action(() => richTextBox2.Text += "\n Среднее число проб в день: " + probemax / daybox));
                ws.Cells[2, 17].Value = probemax/daybox;
                Invoke(new Action(() => richTextBox2.Text += "\n Время в реальности: " + realtime));
                ws.Cells[2, 18].Style.Numberformat.Format = "hh:mm:ss";
                ws.Cells[2, 18].Value = realtime;

                //Способ
                for (int j = 0; j < 27; j++)
                {
                    Invoke(new Action(() => richTextBox2.Text += "\n ------------------------------------------------"));
                    Invoke(new Action(() => richTextBox2.Text += "\n Способ " + (j + 1) + ":"));
                    Invoke(new Action(() => richTextBox2.Text += "\n ------------------------------------------------"));
                    //обнуляем значение среднего
                    average = TimeSpan.Zero;
                    //вычисляем средний результат
                    for (int i = 0; i < times.GetLength(0); i++)
                    {
                        average += TimeSpan.FromSeconds(times[i, j].TotalSeconds);
                    }
                    average = TimeSpan.FromSeconds(average.TotalSeconds / daybox);
                    average = TimeSpan.FromTicks(average.Ticks - (average.Ticks % TimeSpan.TicksPerSecond));
                    //публикуем значение среднего
                    Invoke(new Action(() => richTextBox2.Text += "\n Среднее затраченное время: " + average));
                    ws.Cells[j + 11, 15].Value = j+1;
                    ws.Cells[j+11, 16].Style.Numberformat.Format = "hh:mm:ss";
                    ws.Cells[j + 11, 16].Value = average;
                    Invoke(new Action(() => richTextBox2.Text += "\n ------------------------------------------------"));
                }
                
            
            // Save the Excel file
            Byte[] bin = pack.GetAsByteArray();
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Diploma");
            if (Directory.Exists(path))
            {
                File.WriteAllBytes(System.IO.Path.Combine(path, "report.xlsx"), bin);
            }
            else
            {
                System.IO.Directory.CreateDirectory(path);
                File.WriteAllBytes(System.IO.Path.Combine(path, "report.xlsx"), bin);
            }
            }
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

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void описаниеСпособовToolStripMenuItem_Click(object sender, EventArgs e)
        {

            iForm.Show();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            aboutbox.Show();
        }

        private void задатьКоличествоДнейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputform.Show();
        }

 
    }
}
