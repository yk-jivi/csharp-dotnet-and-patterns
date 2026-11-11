using System.Security.Cryptography;
using System.Text;

namespace Lab5Lib
{
    public interface IWriter
    {
        string? Save(string? message);
    }

    public static class Constant
    {
        public const char Delimiter = '\uffff';
        public const string FileName = "DP.txt";
    }

    public class FileWriter : IWriter
    {
        private string _filename;
        public string FileName
        {
            get => _filename;
        }

        public FileWriter(string? filename = null)
        {
            this._filename = filename ?? Constant.FileName;
        }

        public string? Save(string? message)
        {
            File.WriteAllText(this._filename, message);
            return this._filename;
        }
    }

    public class StrWriter : IWriter
    {
        public string? Save(string? message)
        {
            return message;
        }
    }

    public class Decorator : IWriter
    {
        protected IWriter? _writer; 

        public Decorator(IWriter? writer)
        {
            this._writer = writer;
        }
        public virtual string? Save(string? message)
        {
            return _writer?.Save(message);
        }
    }

    public class DecMD5 : Decorator
    {
        public DecMD5(IWriter writer) : base(writer) { }

        public override string? Save(string? message)
        {
            using (var md5 = MD5.Create())
            {
                var dataBytes = Encoding.UTF8.GetBytes(message ?? string.Empty);
                var encryptedData = md5.ComputeHash(dataBytes);
                var hashedMessage = Convert.ToBase64String(encryptedData);
                var decoratedMessage = $"{message}{Constant.Delimiter}{hashedMessage}";
                return _writer?.Save(decoratedMessage);
            }
        }
    }

    public class DecSHA512 : Decorator
    {
        public DecSHA512(IWriter writer) : base(writer) { }

        public override string? Save(string? message)
        {
            using (var sha512 = SHA512.Create())
            {
                var dataBytes = Encoding.UTF8.GetBytes(message ?? string.Empty);
                var hashedData = sha512.ComputeHash(dataBytes);
                var hashedMessage = Convert.ToBase64String(hashedData);
                var decoratedMessage = $"{message}{Constant.Delimiter}{hashedMessage}";
                return _writer?.Save(decoratedMessage);
            }
        }
    }

    public class DecRSA : Decorator
    {
        public DecRSA(IWriter writer) : base(writer) { }

        public override string? Save(string? message)
        {
            string publicKeyXml;
            byte[] encyptedData;
            message = message ?? string.Empty;
            int k1 = message.IndexOf(Constant.Delimiter);
            if (k1 == -1) throw new Exception("delimiter not found");
            string xs = message.Substring(0, k1);
            string xsbhcs = message.Substring(k1 + 1);
            byte[] xsbhc = Convert.FromBase64String(xsbhcs);
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                publicKeyXml = rsa.ToXmlString(true);
                rsa.ImportParameters(rsa.ExportParameters(false));
                encyptedData = rsa.Encrypt(xsbhc, false);
            }
            string result = $"{xs}{Constant.Delimiter}{Convert.ToBase64String(encyptedData)}{Constant.Delimiter}{publicKeyXml}";
            return _writer?.Save(result);
        }
    }
}
