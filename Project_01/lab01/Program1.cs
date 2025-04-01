namespace Lab2.My.Namespace
{
    public class Program1
    {
        public static void Main(string[] args)
        {
            List<string> list = new List<string>();
            string input;
            while (true) {
                input = Console.ReadLine();
                if (input.Equals("koniec!")) {
                    break;
                }
                list.Add(input);
            }

            list = list.OrderBy(x => x).ToList();
            StreamWriter sw = new StreamWriter("plik1.txt", append:true);
            foreach (var word in list) {
                sw.WriteLine(word);
            }
            sw.Close();
        }
    }
}