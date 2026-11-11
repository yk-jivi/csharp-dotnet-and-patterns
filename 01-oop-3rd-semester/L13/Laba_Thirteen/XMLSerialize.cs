using System.IO;
using System.Xml.Serialization;

namespace Laba_Thirteen
{
    public class XMLSerialize : ISerializer
    {
        public void Serialize<T>(T obj, string file)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var fs = new FileStream(file, FileMode.Create);
            serializer.Serialize(fs, obj);
        }

        public T Deserialize<T>(string file)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var fs = new FileStream(file, FileMode.Open);
            return (T)serializer.Deserialize(fs);
        }
    }
}
