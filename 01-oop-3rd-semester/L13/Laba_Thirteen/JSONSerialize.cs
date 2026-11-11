using System.Text.Json;

namespace Laba_Thirteen
{
    public class JSONSerialize : ISerializer
    {
        public void Serialize<T>(T obj, string file)
        {
            var opt = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(file, JsonSerializer.Serialize(obj, opt));
        }

        public T Deserialize<T>(string file)
        {
            string json = File.ReadAllText(file);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
