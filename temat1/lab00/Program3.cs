using System;
using System.Globalization;

namespace Lab1.My.Namespace
{
    class Program3
    {
        public static void Main(string[] args)
        { 
            double currentNumber;
            double highestNumber = double.MinValue;
            string fileName = args[0];
            StreamReader sr = new StreamReader(fileName);
            int currentLineNumber = 0;
            List<int> highestLineNumbers = new List<int>();

            while (!sr.EndOfStream)
            {
                currentLineNumber++;
                String line = sr.ReadLine();
                if (!double.TryParse(line, new CultureInfo("en-US"), out currentNumber))
                {
                    break;
                }

                if (currentNumber > highestNumber)
                {
                    highestNumber = currentNumber;
                    highestLineNumbers.Clear();
                    highestLineNumbers.Add(currentLineNumber);
                }
                else if (currentNumber == highestNumber)
                {
                    highestLineNumbers.Add(currentLineNumber);
                }
            } 
            sr.Close();
            string lineNumbers = string.Join(", ", highestLineNumbers);
            Console.WriteLine($"{highestNumber}, linijka: {lineNumbers}");

        }
    }
}