using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lab03
{
    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "MMMM dd, yyyy 'at' hh:mmtt";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? dateString = reader.GetString();
            if (dateString == null)
            {
                throw new JsonException("Nie ma daty w strumieniu JSON");
            }

            try
            {
                return DateTime.ParseExact(dateString, DateFormat, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                throw new JsonException($"Nieprawidlowy format daty: {dateString}", e);
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
        
        
    }
}