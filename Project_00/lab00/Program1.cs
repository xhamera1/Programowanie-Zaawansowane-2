namespace Lab1.My.Namespace
{
 
    class Program1
    {
        public static void Main(string[] args)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                for (int j = 0; j < Int32.Parse(args[args.Length - 1]); j++)
                {
                    Console.WriteLine(args[i]);
                }
            }
        }
    }
}


