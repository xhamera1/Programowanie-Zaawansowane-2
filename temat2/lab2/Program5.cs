using System;
using System.IO;

namespace Lab2.My.Namespace
{
    class Program4
    {
        private const string END_OF_RUN = "<<<END_OF_RUN>>>";

        static void Main(string[] args)
        {
            // Dla przykładu: nazwy plików wejściowych i pomocniczych
            string inputFile = "input.txt";
            string runAFile = "runA.txt";
            string runBFile = "runB.txt";
            string mergedFile = "merged.txt";

            // Główna pętla: rozdzielanie -> (jeśli potrzeba) scalanie -> powtarzanie
            // Dopóki w pliku wejściowym jest więcej niż 1 seria
            while (true)
            {
                // 1) Rozdziel dane na dwa pliki pomocnicze
                int seriesCount = DistributeSeries(inputFile, runAFile, runBFile);

                // Jeśli w pliku jest 0 lub 1 seria, to znaczy, że już nie trzeba sortować
                if (seriesCount <= 1)
                {
                    Console.WriteLine("Plik jest już w pełni posortowany lub pusty.");
                    break;
                }

                // 2) Scalaj pary serii z runA i runB do mergedFile
                MergeSeries(runAFile, runBFile, mergedFile);

                // 3) Skopiuj wynik scalania z powrotem do pliku wejściowego
                File.Copy(mergedFile, inputFile, overwrite: true);
            }

            Console.WriteLine("Sortowanie zakończone. Wynik w pliku: " + inputFile);
        }

        /// <summary>
        /// Faza rozdzielania: wczytuje (sekwencyjnie) plik wejściowy,
        /// wykrywa "naturalne" serie niemalejące i zapisywuje je
        /// naprzemiennie do dwóch plików pomocniczych: runA i runB.
        /// Na koniec każdej serii zapisuje znacznik końca serii.
        /// Zwraca liczbę utworzonych serii.
        /// </summary>
        static int DistributeSeries(string inputFile, string runAFile, string runBFile)
        {
            // Zabezpieczamy: jeżeli plik wejściowy nie istnieje lub jest pusty
            if (!File.Exists(inputFile))
            {
                // Wyczyszczamy pliki pomocnicze
                File.WriteAllText(runAFile, string.Empty);
                File.WriteAllText(runBFile, string.Empty);
                return 0;
            }

            int seriesCount = 0;
            bool writeToA = true; // flaga: czy aktualną serię piszemy do runA czy runB
            string previousLine = null;
            bool firstLine = true;

            using (var sr = new StreamReader(inputFile))
            using (var swA = new StreamWriter(runAFile, false))
            using (var swB = new StreamWriter(runBFile, false))
            {
                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                    {
                        // koniec pliku
                        // Jeśli była już jakakolwiek linia, trzeba domknąć serię
                        if (!firstLine)
                        {
                            seriesCount++;
                            // Domykamy serię w odpowiednim pliku
                            if (writeToA) swA.WriteLine(END_OF_RUN);
                            else swB.WriteLine(END_OF_RUN);
                        }
                        break;
                    }

                    // Czy to pierwsza linia w ogóle?
                    if (firstLine)
                    {
                        firstLine = false;
                        seriesCount = 1; // startujemy pierwszą serię
                        if (writeToA) swA.WriteLine(line);
                        else swB.WriteLine(line);

                        previousLine = line;
                    }
                    else
                    {
                        // Sprawdzamy, czy linia kontynuuje naturalnie tę samą serię
                        if (string.Compare(line, previousLine, StringComparison.Ordinal) >= 0)
                        {
                            // Należy do tej samej serii
                            if (writeToA) swA.WriteLine(line);
                            else swB.WriteLine(line);

                            previousLine = line;
                        }
                        else
                        {
                            // Koniec aktualnej serii - domykamy ją
                            if (writeToA) swA.WriteLine(END_OF_RUN);
                            else swB.WriteLine(END_OF_RUN);

                            seriesCount++;
                            // Przełączamy się na drugi plik
                            writeToA = !writeToA;

                            // Rozpoczynamy nową serię, zapiszmy aktualną linię
                            if (writeToA) swA.WriteLine(line);
                            else swB.WriteLine(line);

                            previousLine = line;
                        }
                    }
                }
            }

            return seriesCount;
        }

        /// <summary>
        /// Faza scalania: pliki runA i runB zawierają na przemian serie
        /// zakończone znacznikiem END_OF_RUN. Funkcja scala pary serii
        /// (pierwszą z runA z pierwszą z runB, drugą z runA z drugą z runB itd.)
        /// i zapisuje wyniki do mergedFile (każda połączona seria jest też
        /// zakończona znacznikiem END_OF_RUN).
        /// </summary>
        static void MergeSeries(string runAFile, string runBFile, string mergedFile)
        {
            using (var srA = new StreamReader(runAFile))
            using (var srB = new StreamReader(runBFile))
            using (var swMerged = new StreamWriter(mergedFile, false))
            {
                bool endOfA = false;
                bool endOfB = false;

                while (true)
                {
                    // Wczytujemy jedną serię z pliku A i jedną serię z pliku B
                    // do scalania (ale bez wczytywania wszystkiego naraz w pamięć –
                    // będziemy czytać "na bieżąco" linia-po-linii, porównywać i zapisywać).

                    // Próba rozpoczęcia serii z A:
                    if (endOfA && endOfB)
                    {
                        // Oba pliki wyczerpane całkowicie - kończymy
                        break;
                    }
                    else if (endOfA && !endOfB)
                    {
                        // Plik A już się skończył - przepisujemy wszystkie pozostałe serie z B
                        CopyRemainingSeries(srB, swMerged);
                        break;
                    }
                    else if (!endOfA && endOfB)
                    {
                        // Plik B się skończył - przepisujemy wszystkie pozostałe serie z A
                        CopyRemainingSeries(srA, swMerged);
                        break;
                    }

                    // Wczytujemy (sekwencyjnie) i scalamy jedną serię z A i jedną serię z B
                    bool runAEmpty = false;
                    bool runBEmpty = false;
                    string currentA = ReadNextLineOfRun(srA, ref runAEmpty, ref endOfA);
                    string currentB = ReadNextLineOfRun(srB, ref runBEmpty, ref endOfB);

                    // Jeśli w A albo w B nie było już żadnej serii – przechodzimy do kopiowania
                    if (currentA == null && currentB == null)
                    {
                        // Oba już puste - pewnie koniec
                        // Ale wypiszmy znacznik serii, żeby ładnie domknąć
                        swMerged.WriteLine(END_OF_RUN);
                        continue;
                    }
                    else if (currentA == null && currentB != null)
                    {
                        // Seria w A pusta, w B jest – przepisujemy tę serię z B
                        while (currentB != null)
                        {
                            swMerged.WriteLine(currentB);
                            currentB = ReadNextLineOfRun(srB, ref runBEmpty, ref endOfB);
                        }
                        swMerged.WriteLine(END_OF_RUN);
                        continue;
                    }
                    else if (currentA != null && currentB == null)
                    {
                        // Analogicznie, w B pusto, a w A coś jest
                        while (currentA != null)
                        {
                            swMerged.WriteLine(currentA);
                            currentA = ReadNextLineOfRun(srA, ref runAEmpty, ref endOfA);
                        }
                        swMerged.WriteLine(END_OF_RUN);
                        continue;
                    }

                    // Scalanie linia po linii (obie serie A i B mają jakieś dane):
                    while (currentA != null && currentB != null)
                    {
                        // Porównujemy i wypisujemy mniejszą
                        if (string.Compare(currentA, currentB, StringComparison.Ordinal) <= 0)
                        {
                            swMerged.WriteLine(currentA);
                            currentA = ReadNextLineOfRun(srA, ref runAEmpty, ref endOfA);
                        }
                        else
                        {
                            swMerged.WriteLine(currentB);
                            currentB = ReadNextLineOfRun(srB, ref runBEmpty, ref endOfB);
                        }
                    }

                    // Jeśli w którejś serii jeszcze coś zostało, to dopiszmy resztę
                    while (currentA != null)
                    {
                        swMerged.WriteLine(currentA);
                        currentA = ReadNextLineOfRun(srA, ref runAEmpty, ref endOfA);
                    }
                    while (currentB != null)
                    {
                        swMerged.WriteLine(currentB);
                        currentB = ReadNextLineOfRun(srB, ref runBEmpty, ref endOfB);
                    }

                    // Koniec scalania jednej pary serii – dopisujemy znacznik
                    swMerged.WriteLine(END_OF_RUN);
                }
            }
        }

        /// <summary>
        /// Czyta z podanego strumienia kolejną linię należącą do BIEŻĄCEJ serii
        /// (czyli aż do znaku END_OF_RUN lub końca pliku).
        /// - Zwraca tę linię lub null, jeżeli dana seria się skończyła.
        /// - runEmpty = true, jeśli seria w pliku wyczerpała się i nie ma już kolejnej.
        /// - endOfFile = true, jeśli cały plik został wyczerpany (nie ma już nawet kolejnych serii).
        /// </summary>
        static string ReadNextLineOfRun(StreamReader sr, ref bool runEmpty, ref bool endOfFile)
        {
            if (endOfFile) return null;  // jeśli już wiemy, że plik się skończył

            while (true)
            {
                var line = sr.ReadLine();
                if (line == null)
                {
                    // koniec pliku
                    endOfFile = true;
                    return null;
                }
                if (line == END_OF_RUN)
                {
                    // koniec bieżącej serii
                    runEmpty = true;
                    return null;
                }

                // Zwracamy normalną linię (zawartą w serii)
                // Po wyjściu stąd kolejny odczyt sam się okaże, czy seria trwa.
                runEmpty = false;
                return line;
            }
        }

        /// <summary>
        /// Kopiuje wszystkie pozostałe serie z pliku źródłowego do pliku docelowego,
        /// łącznie z liniami i znacznikami END_OF_RUN (tak, by zachować podział na serie).
        /// Używane w przypadku, gdy drugi plik już się wyczerpał i nie ma z czym scalać.
        /// </summary>
        static void CopyRemainingSeries(StreamReader srSource, StreamWriter swDest)
        {
            while (!srSource.EndOfStream)
            {
                string line = srSource.ReadLine();
                if (line != null)
                {
                    swDest.WriteLine(line);
                }
            }
        }
    }
}
