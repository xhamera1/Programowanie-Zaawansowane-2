namespace Lab2.My.Namespace{
    class Program3{
        static void Main(string[] args){
            if (args.Length != 6){
                Console.WriteLine("Niepoprawna ilsoc argumentow programu");
                return;
            }
            string filename = args[0];
            int n = int.Parse(args[1]);
            double od = double.Parse(args[2]);
            double doz = double.Parse(args[3]);
            int seed = int.Parse(args[4]);
            string realOrInt = args[5];
            bool isReal;
            if (realOrInt.Equals("real")){
                isReal = true;
            }
            else {
                isReal = false;
            }

            StreamWriter sw = new StreamWriter(filename, false);
            Random random = new Random(seed);
            for (int i = 0; i < n; i++){
                if (isReal) {
                    double value = od + random.NextDouble() * (doz - od);
                    sw.WriteLine(value);
                }
                else {
                    int value = random.Next((int)od, (int)doz);
                    sw.WriteLine(value);
                }
            }
            sw.Close();
        }
    }

}