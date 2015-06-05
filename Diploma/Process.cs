using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma
{
    class Process
    {
        //массив для хранения значений времени для каждого способа в течение одного цикла
        TimeSpan[] time { get; set; }
        //массив общего времени для каждого дня и каждого способа
        TimeSpan[,] times { get; set; }
        //переменная для расчета среднего значения времени
        TimeSpan average { get; set; }
        //массив времени аппаратов
        TimeSpan[][] timeMachine = new TimeSpan[3][]
            {
                new TimeSpan[4],
                new TimeSpan[4],
                new TimeSpan[4],
            };
    }
}
