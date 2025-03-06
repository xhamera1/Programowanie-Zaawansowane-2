using System;
using System.Collections.Generic;


namespace Lab1.My.Namespace
{
    class Program4
    {
        public static void Main(string[] args)
        {
            List<string> listSounds = new List<string>(new string[]
            {
                "C","C#","D","D#","E","F","F#","G","G#","A","B","H"
            });

            string input = Console.ReadLine();
            int index = listSounds.IndexOf(input);
            if (index == -1)
            {
                Console.WriteLine("nie ma takiego dzwieku");
            }
            else
            {
                List<string> resultGama = resultList(listSounds, index);
                foreach (var soundGama in resultGama)
                {
                    Console.Write($"{soundGama} ");
                }
            }
            
        }

        
        
        
        private static List<string> resultList(List<string> listSounds, int index)
        { 
            int[] scale = {2,2,1,2,2,2,1};
            List<string> resultGama = new List<string>();
            foreach (int step in scale)
            {
                resultGama.Add(listSounds[index % listSounds.Count]);
                index += step;
            }
            resultGama.Add(listSounds[index % listSounds.Count]);

            return resultGama;
        }
    }
}