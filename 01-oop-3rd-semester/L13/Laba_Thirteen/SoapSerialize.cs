using System.IO;
using System.Runtime.Serialization;

namespace Laba_Thirteen
{
    public class SoapSerialize : ISerializer
    {
        public void Serialize<T>(T obj, string file)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using FileStream fs = new FileStream(file, FileMode.Create);
            serializer.WriteObject(fs, obj);
        }
        public T Deserialize<T>(string file)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using FileStream fs = new FileStream(file, FileMode.Open);
            return (T)serializer.ReadObject(fs);
        }
    }
}
