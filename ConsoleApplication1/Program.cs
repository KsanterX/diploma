using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //создаём список диапазонов и вероятностей
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
                                Console.WriteLine(selectedElement);
                                Console.WriteLine("Точнее 20,2-40,4");
                                break;
                            case 2:
                                selectedElement = elements[i].Key;
                                Console.WriteLine(selectedElement);
                                Console.WriteLine("Точнее 80,8-101");
                                break;
                            default:
                                selectedElement = elements[i].Key;
                                Console.WriteLine(selectedElement);
                                Console.WriteLine("Точнее 181,8-202");
                                break;
                        }
                    }
                    else
                    {
                        //иначе указываем диапазон, в который попадает сгенерированное число
                        selectedElement = elements[i].Key;
                        Console.WriteLine(selectedElement);
                        break;
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
