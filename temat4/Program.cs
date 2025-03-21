using System.Text.RegularExpressions;

namespace Lab03
{
    class Program
    {
        public static void Main(string[] args)
        {
            // ZADANIE 1
            JsonTweetHandler jsonTweetHandler = new JsonTweetHandler("favorite-tweets.jsonl");
            List<Tweet>? tweets1 = jsonTweetHandler.GetListOfTweets();

            Console.WriteLine("");
            Console.WriteLine("----------------- Lista tweetow z json ---------------------------");
            Console.WriteLine("");
            
            
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(tweets1?[i]);
            }

            // ZADANIE 2
            string filenameXml = "tweetsFromListXML.xml";
            jsonTweetHandler.ConvertListTweetsToXml(tweets1, filenameXml);

            List<Tweet>? tweets2 = jsonTweetHandler.GetListOfTweetsFromXml(filenameXml);
            
            Console.WriteLine("");
            Console.WriteLine("-------------------- Lista tweetow z xml ------------------------");
            Console.WriteLine("");
            
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(tweets2?[i]);
            }

            Console.WriteLine("");
            Console.WriteLine("----------------- Posortowana lista wedlug username : ---------------------------");
            Console.WriteLine("");
            
            // SortTweetsByUsername(tweets1);
            //
            // for (int i = 0; i < 3; i++)
            // {
            //     Console.WriteLine(tweets1?[i].UserName);
            // }
            
            // ZADANIE 3 po username
            
            SortedList<string, Tweet> sortedListByUsername = GetSortedListOfTweetsByUsername(tweets1);

            int x = 0;
            foreach (var tuple in sortedListByUsername)
            {
                Console.WriteLine(tuple.Value.UserName);
                if (x == 10) break;
                x++;

            }
            
            
            
            
            Console.WriteLine("");
            Console.WriteLine("----------------- Posortowana lista wedlug daty utowrzneia tweeta : ---------------------------");
            Console.WriteLine("");

            // SortTweetsByCreatedAt(tweets1);
            //
            // for (int i = 0; i < 10; i++)
            // {
            //     Console.WriteLine(tweets1?[i].CreatedAt);
            // }
            
            // ZADANIE 3 po createdat
            
            SortedList<DateTime, Tweet> sortedListByCreatedAt = GetSortedListOfTweetsByCreatedAt(tweets1);

            x = 0;
            foreach (var tuple in sortedListByCreatedAt)
            {
                Console.WriteLine(tuple.Value.CreatedAt);
                if (x == 10) break;
                x++;

            }
            
            Console.WriteLine("");
            Console.WriteLine("----------------- Najnowszy i najstarszy tweet: : ---------------------------");
            Console.WriteLine("");
            
            // ZADANIE 4
            
            Console.WriteLine($"Najstarszy tweet: {sortedListByCreatedAt.GetValueAtIndex(0)}");

            Console.WriteLine("");
            
            int len = sortedListByUsername.Count; 
            Console.WriteLine($"Najnowszy tweet: {sortedListByCreatedAt.GetValueAtIndex(len-1)}");
            
            Console.WriteLine("");
            Console.WriteLine("----------------- Slownik indeksowany po username : ---------------------------");
            Console.WriteLine("");

            // ZADANIE 5

            x = 0;
            Dictionary<string, List<Tweet>> tweetsDictionary = GetTweetsDictionary(sortedListByUsername);
            foreach (var pair in tweetsDictionary)
            {
                List<Tweet> list = pair.Value;
                Console.Write(pair.Key + ":  ");
                foreach (var tweet in list)
                {
                    Console.WriteLine(tweet);
                }
                if (x == 5) break;
                x++;
            }
            
            Console.WriteLine("");
            Console.WriteLine("----------------- Slownik czestosci wystepowania slow : ---------------------------");
            Console.WriteLine("");
            
            // ZADANIE 6


            Dictionary<string, int> wordsDictFreq = GetWordFrequencies(tweets1);

            x = 0;
            foreach (var pair in wordsDictFreq)
            {
                Console.WriteLine(pair.Key + " : " + pair.Value);
                if (x == 5) break;
                x++;
            }
            
            Console.WriteLine("");
            Console.WriteLine("----------------- 10 najczestszych slow o dlugosci co najmniej 5 : ---------------------------");
            Console.WriteLine("");

            // ZADANIE 7
            
            var dictSortedPairLength5 = wordsDictFreq.Where(pair => pair.Key.Length >= 5)
                .OrderByDescending(pair => pair.Value);

            x = 0;
            foreach (var p in dictSortedPairLength5) 
            {
                Console.WriteLine(p.Key + " : " + p.Value);
                if (x == 9) break;
                x++;
            }
            
            Console.WriteLine("");
            Console.WriteLine("----------------- IDF dla wszystkich slow ---------------------------");
            Console.WriteLine("");
            
            // ZADANIE 8 

            Dictionary<string, double> dictionaryIDF = GetDictionaryIDF(tweets1);

            x = 0;
            foreach (var pair in dictionaryIDF)
            {
                Console.WriteLine(pair.Key + " : " + pair.Value);
                if (x == 5) break;
                x++;
            }
            
            Console.WriteLine("");
            Console.WriteLine("----------------- IDF posortowane malejaco, 10 wurazow z najwiekszyym IDF: ---------------------------");
            Console.WriteLine("");

            var sortedDictIdfTop10 = dictionaryIDF.OrderByDescending(pair => pair.Value);

            x = 0;
            foreach (var pair in sortedDictIdfTop10)
            {
                Console.WriteLine(pair.Key + " : " + pair.Value);
                if (x == 9) break;
                x++;
            }



        }


        public static SortedList<string, Tweet> GetSortedListOfTweetsByUsername(List<Tweet>? tweets)
        {
            if (tweets == null)
                return new SortedList<string, Tweet>();
            SortedList<string, Tweet>? sortedList = new SortedList<string, Tweet>();

            foreach (Tweet? tweet in tweets)
            {
                string? baseKey = tweet.UserName;
                string? key = baseKey ?? "";
                int counter = 1;
                while (sortedList.ContainsKey(key))
                {
                    key = $"{baseKey}_{counter}";
                    counter++;
                }
                sortedList.Add(key, tweet);
            }

            return sortedList;
        }
        
        public static SortedList<DateTime, Tweet> GetSortedListOfTweetsByCreatedAt(List<Tweet>? tweets)
        {
            if (tweets == null)
                return new SortedList<DateTime, Tweet>();
            SortedList<DateTime, Tweet>? sortedList = new SortedList<DateTime, Tweet>();

            foreach (Tweet? tweet in tweets)
            {
                DateTime key = tweet.CreatedAt;
                while (sortedList.ContainsKey(key))
                {
                    key = key.AddMilliseconds(1);
                }
                sortedList.Add(key, tweet);
            }

            return sortedList;
        }


        public static Dictionary<string, List<Tweet>> GetTweetsDictionary(SortedList<string, Tweet> sortedList)
        {
            Dictionary<string, List<Tweet>> tweetsDictionary = new Dictionary<string, List<Tweet>>();
            foreach (var tuple in sortedList )
            {
                if (tweetsDictionary.ContainsKey(tuple.Key))
                {
                    tweetsDictionary[tuple.Key].Add(tuple.Value);
                }
                else
                {
                    tweetsDictionary.Add(tuple.Key, new List<Tweet>{tuple.Value});
                }
            }
            return tweetsDictionary;
        }
        
        public static Dictionary<string, int> GetWordFrequencies(List<Tweet>? tweets)
        {
            Dictionary<string, int>? wordsFrequencies = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (Tweet? tweet in tweets)
            {
                if (!string.IsNullOrEmpty(tweet.Text))
                {
                    string[] words = Regex.Split(tweet.Text,
                        @"\W+"); // jeden lub weicej znakow nie bedacych literami/cyframi/podkresleniem
                    foreach (string? word in words)
                    {
                        if (!string.IsNullOrEmpty(word))
                        {
                            if (wordsFrequencies.ContainsKey(word))
                            {
                                wordsFrequencies[word]++;
                            }
                            else
                            {
                                wordsFrequencies[word] = 1;
                            }
                        }
                    }
                }
            }

            return wordsFrequencies;
        }


        public static Dictionary<string, double> GetDictionaryIDF(List<Tweet>? tweets)
        {
            // IDF(s) = ln(liczba tweetow / liczba tweetow zawierajacych slowo s)

            Dictionary<string, double> dictionaryIDF = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            Dictionary<string, int> dictFreq = new Dictionary<string, int>();

            int n = tweets!.Count;
            
            foreach (Tweet tweet in tweets)
            {
                if (string.IsNullOrEmpty(tweet.Text))
                {
                    continue;
                }

                var uniqueWords = Regex.Split(tweet.Text, @"\W+")
                    .Where(word => !string.IsNullOrEmpty(word))
                    .Select(word => word.ToLower())
                    .Distinct();

                foreach (string word in uniqueWords)
                {
                    if (dictFreq.ContainsKey(word))
                    {
                        dictFreq[word]++;
                    }
                    else
                    {
                        dictFreq[word] = 1;
                    }
                }
            }

            foreach (var pair in dictFreq)
            {
                double idf = Math.Log((double)n/ pair.Value);
                dictionaryIDF[pair.Key] = idf;
            }

            return dictionaryIDF;
        }


        // public static void SortTweetsByUsername(List<Tweet>? tweets)
        // {
        //     tweets?.Sort(CompareByUsername);
        // }
        //
        // public static int CompareByUsername(Tweet? t1, Tweet? t2)
        // {
        //     return string.Compare(t1?.UserName ?? "", t2?.UserName ?? "");
        // }
        //
        // public static void SortTweetsByCreatedAt(List<Tweet>? tweets)
        // {
        //     tweets?.Sort(CompareByCreatedAt);
        // }
        //
        // public static int CompareByCreatedAt(Tweet? t1, Tweet? t2)
        // {
        //     if (t1 is null && t2 is null) return 0;
        //     if (t1 is null) return -1;
        //     if (t2 is null) return 1;
        //     return DateTime.Compare(t1.CreatedAt, t2.CreatedAt);
        // }
    }
}