using System;
using System.Text.Json.Serialization;

namespace Lab03
{
    public class Tweet
    {
        public string? Text { get; set; }
        public string? UserName { get; set; }
        public string? LinkToTweet { get; set; }
        public string? FirstLinkUrl { get; set; }
        
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime CreatedAt { get; set; }
        
        public string? TweetEmbedCode { get; set; }
        
        public override string ToString()
        {
            return $"User: {UserName}, Text: {Text}, Created At: {CreatedAt}";
        }
        
    }
} 

