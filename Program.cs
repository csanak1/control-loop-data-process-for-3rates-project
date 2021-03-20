using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLoopProcessFor3Rates
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("     +++ BorsodChem Zrt. +++      ");
            //Console.WriteLine("3Rates Project - Control loops shiftly data query");
            //Console.WriteLine("");

            InitializeShiftDateTime(out DateTime queryStart, out DateTime queryEnd);

            //Console.WriteLine("--Collection started for the previous shift");
            //Console.WriteLine("---Collection interval: " + queryStart.ToString("yyyy.MM.dd. HH:mm") + " - " + queryEnd.ToString("yyyy.MM.dd. HH:mm"));

            var handler = new Handler();

            handler.ProcessCRData(queryStart, queryEnd); //CR of control loops
        }

        private static void InitializeShiftDateTime(out DateTime queryStart, out DateTime queryEnd)
        {
            string tempDate = DateTime.Now.ToString("yyyy.MM.dd.");
            int currentHour = Convert.ToInt16(DateTime.Now.Hour);

            if (currentHour >= 6 && currentHour < 14) //22:00 shift
            {
                queryEnd = Convert.ToDateTime(tempDate + " 6:00"); //query end
                queryStart = queryEnd.AddHours(-8); //query start

            }
            else if (currentHour >= 14 && currentHour < 22) //6:00 shift
            {
                queryEnd = Convert.ToDateTime(tempDate + " 14:00");
                queryStart = queryEnd.AddHours(-8);
            }
            else if (currentHour >= 22) //14:00 shift
            {
                queryEnd = Convert.ToDateTime(tempDate + " 22:00");
                queryStart = queryEnd.AddHours(-8);
            }
            else //14:00 shift as well
            {
                queryEnd = Convert.ToDateTime(tempDate + " 22:00");
                queryStart = queryEnd.AddHours(-8);
            }
        }
    }
}
