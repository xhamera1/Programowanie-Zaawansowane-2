using System;
using System.Globalization;

namespace Lab1.My.Namespace
{
    class Program2
    { 
        public static void Main(string[] args)
        {
            double average;
            double sum = 0;
            double number;
            int count = 0;

            while(true)
            {
                string input = Console.ReadLine();
                if (!double.TryParse(input, new CultureInfo("en-US"), out number))
                {
                    break;
                }
                if (number == 0)
                {
                    break;
                }
                sum += number;
                count++;

            }

            average = sum / count;
            StreamWriter sw = new StreamWriter("wyniki2.txt", true);
            sw.WriteLine(average.ToString());
            sw.Close();

        }
    }
}

