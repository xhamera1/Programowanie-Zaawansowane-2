using System.Text.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Lab03
{
    public class JsonTweetHandler
    {
        private string? fileURL;

        public JsonTweetHandler(string fileURL)
        {
            this.fileURL = fileURL;
        }

        public List<Tweet>? GetListOfTweets()
        {
            String[]? jsonLines = File.ReadAllLines(this.fileURL ?? string.Empty);
            List<Tweet> tweets = new List<Tweet>();
            foreach (String line in jsonLines)
            {
                try
                {
                    Tweet? tweet = JsonSerializer.Deserialize<Tweet>(line);
                    if (tweet != null)
                    {
                        tweets.Add(tweet);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while deserializing line: {line}\n{ex.Message}");
                }
            }

            return tweets;
        }

        public void ConvertListTweetsToXml(List<Tweet>? tweets, string filenameXML)
        {
            if (tweets == null)
                return;
        
            XmlSerializer serializer = new XmlSerializer(typeof(List<Tweet>));
            using (StreamWriter writer = new StreamWriter(filenameXML))
            {
                serializer.Serialize(writer, tweets);
            }
        }

        public List<Tweet>? GetListOfTweetsFromXml(string xmlFilePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Tweet>));
            using (StreamReader reader = new StreamReader(xmlFilePath))
            {
                return (List<Tweet>?)serializer.Deserialize(reader);
            }
        }
        
    }
}