namespace Lab2.My.Namespace{
    class Program2{
        static void Main(string[] args){
            if (args.Length != 2){
                Console.WriteLine("Niepoprawna liczba argumentow");
                return;
            }
            // nazwa pliku do tego zadania to plik2.txt
            string filename = args[0];
            string word = args[1];

            List<string> results = new List<string>();

            StreamReader sr = new StreamReader(filename);
            int lineNumber = 1;

            while(!sr.EndOfStream){
                String napis = sr.ReadLine();
                int index = 0;
                while ((index = napis.IndexOf(word, index)) != -1){
                    results.Add($"linijka: {lineNumber}, pozycja: {index}");
                    index++;
                }
                lineNumber++;
            
            }
            sr.Close();

            foreach (string result in results){
                Console.WriteLine(result);
            }


        }
    }
}