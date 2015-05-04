using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma
{
    static class Program
    {
        //Form access
        static MainForm MyForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MyForm = new MainForm();
            Application.Run(MyForm);
            
        }

        /*/список названий исследований и их id
        List<KeyValuePair<string, int>> elements = new List<KeyValuePair<string, int>>
                                                          {
                                                              new KeyValuePair<string, int>("ПТИ", 1),
                                                              new KeyValuePair<string, int>("К,NA", 2),
                                                              new KeyValuePair<string, int>("СРБ ,АСо", 3),
                                                              new KeyValuePair<string, int>("Глюкоза", 4),
                                                              new KeyValuePair<string, int>("Холестерин", 5),
                                                              new KeyValuePair<string, int>("Бетта-липопротеиды,триглицириды", 6),
                                                              new KeyValuePair<string, int>("ЛПНП,ЛПВП", 7),
                                                              new KeyValuePair<string, int>("Билирубин общий", 8),
                                                              new KeyValuePair<string, int>("Билирубин прямой", 9),
                                                              new KeyValuePair<string, int>("АЛТ, АСТ", 10),
                                                              new KeyValuePair<string, int>("Тимоловая проба", 11),
                                                              new KeyValuePair<string, int>("Мочевина, Креатинин", 12),
                                                              new KeyValuePair<string, int>("Общий белок", 13),
                                                              new KeyValuePair<string, int>("Fe, ЖСС", 14),
                                                              new KeyValuePair<string, int>("Альфа амил", 15),
                                                              new KeyValuePair<string, int>("Мочевая кислота", 16),
                                                              new KeyValuePair<string, int>("Щелочной фосфотазы", 17),
                                                              new KeyValuePair<string, int>("ГГТП", 18)
                                                          };
        создаём список диапазонов и вероятностей
           List<KeyValuePair<string, double>> elements = new List<KeyValuePair<string, double>>
                                                         {
                                                             new KeyValuePair<string, double>("20,2-40,4 или 80,8-101 или 181,8-202", 0.014),
                                                             new KeyValuePair<string, double>("60,6-80,8", 0.043),
                                                             new KeyValuePair<string, double>("161,6-181,8", 0.057),
                                                             new KeyValuePair<string, double>("141,4-161,6", 0.100),
                                                             new KeyValuePair<string, double>("101-121,2", 0.157),
                                                             new KeyValuePair<string, double>("121,2-141,4", 0.186),
                                                             new KeyValuePair<string, double>("0-20.2", 0.414)
                                                         };
           string selectedElement;

           //генератор
           Random r = new Random();
           double diceRoll = r.NextDouble();

           //переменная для подсчета кумулятивной вероятности
           double cumulative = 0.0;

           //цикл подсчета кумулятивной вероятности и сравнения с вероятностью из списка
           for (int i = 0; i < elements.Count; i++)
           {
               //кумулятивная вероятность = кумулятивная вероятность + вероятность элемента списка
               cumulative += elements[i].Value;

               //если сгенерированная вероятность меньше кумулятивной вероятности элемента списка
               if (diceRoll < cumulative)
               {
                   //добавил дополнительную проверку, поскольку три элемента в списке имеют одинаковую вероятность
                   if (elements[i].Value == 0.014)
                   {
                       //генерируем новое число от 1 до 3 и выбираем какому элементу присвоить выпадение
                       Random a = new Random(DateTime.Now.Millisecond);
                       int ind = a.Next(1, 3);
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
           } */

        
    }
}
