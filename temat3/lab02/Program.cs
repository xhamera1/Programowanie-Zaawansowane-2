#nullable disable

namespace Lab02
{
    class Program
    {
        public static void Main(string[] args)
        {
            OsobaFizyczna osoba1 = new OsobaFizyczna("Jan", "Kowalski", "Adam",
                "11111111111", "1111");
            RachunekBankowy rachunekBankowy1 = new RachunekBankowy("1", 1000m, true,
                new List<PosiadaczRachunku> { osoba1 });
            
            OsobaFizyczna osoba2 = new OsobaFizyczna("Adam", "Nowak", "Michal",
                "11112222222", "2222");
            RachunekBankowy rachunekBankowy2 = new RachunekBankowy("2", 5000m, false,
                new List<PosiadaczRachunku> { osoba2 });
            
            RachunekBankowy.DokonajTransakcji(rachunekBankowy1, rachunekBankowy2, 6000m, "Przelew");


            Console.WriteLine("Stan konta 1: " + rachunekBankowy1);
            Console.WriteLine("Stan konta 2: " + rachunekBankowy2);
            
            OsobaFizyczna osoba3 = new OsobaFizyczna("Michal", "Burek", "Adam", "11113333333", "3333");
            rachunekBankowy1 += osoba3;

            Console.WriteLine(rachunekBankowy1);


        }

    }
}

