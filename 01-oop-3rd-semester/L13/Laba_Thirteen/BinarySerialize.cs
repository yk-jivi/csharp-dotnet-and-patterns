using System.IO;
using System.Runtime.Serialization;

namespace Laba_Thirteen
{
    public class BinarySerialize : ISerializer
    {
        public void Serialize<T>(T obj, string file)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            File.WriteAllBytes(file, ms.ToArray());
        }
        public T Deserialize<T>(string file)
        {
            byte[] data = File.ReadAllBytes(file);
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using MemoryStream ms = new MemoryStream(data);
            return (T)serializer.ReadObject(ms);
        }
    }
}
