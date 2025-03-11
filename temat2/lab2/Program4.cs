namespace Lab2.My.Namespace{
    class Program4{
        static void Main(string[] args){
            int lineCount = File.ReadAllLines("plik3.txt").Length;
            int countChars = 0;
            double biggestNumber = double.MinValue;
            double smallestNumber = double.MaxValue;
            double sum = 0;
            double average = 0;
            StreamReader sr = new StreamReader("plik3.txt");
            while (!sr.EndOfStream) {
                string line = sr.ReadLine();
                countChars += line.Length;
                double number = double.Parse(line);
                if (number >= biggestNumber) {
                    biggestNumber = number;
                }
                if  (number <= smallestNumber) {
                    smallestNumber = number;
                }
                sum += number;
            }
            if (lineCount != 0) {
                average = sum / lineCount;
            }
            Console.WriteLine("Liczba linii pliku: " + lineCount);
            Console.WriteLine("Liczba znaków w pliku: " + countChars);
            Console.WriteLine("Najwieksza liczba: " + biggestNumber);
            Console.WriteLine("Najmniejsza liczba: " + smallestNumber);
            Console.WriteLine("Srednia liczb: " + average);
            sr.Close();
 
        }
    }
}