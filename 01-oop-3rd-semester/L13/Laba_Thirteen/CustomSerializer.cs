namespace Laba_Thirteen
{
    public enum SerializationType { Binary, Soap, Xml, Json }

    public class CustomSerializer
    {
        private readonly ISerializer serializer;

        public CustomSerializer(SerializationType type)
        {
            serializer = type switch
            {
                SerializationType.Binary => new BinarySerialize(),
                SerializationType.Soap => new SoapSerialize(),
                SerializationType.Xml => new XMLSerialize(),
                SerializationType.Json => new JSONSerialize(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Save<T>(T obj, string file) => serializer.Serialize(obj, file);
        public T Load<T>(string file) => serializer.Deserialize<T>(file);
    }
}
