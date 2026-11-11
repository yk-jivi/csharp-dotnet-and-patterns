using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml.Serialization;

namespace Laba2
{
    [Serializable]
    public class Processor
    {
        public string Manufacturer { get; set; }
        public string Series { get; set; }
        public string Model { get; set; }
        public int Cores { get; set; }
        public double Frequency { get; set; }
        public double MaxFrequency { get; set; }
        public int Architecture { get; set; }
        public int L1Cache { get; set; }
        public int L2Cache { get; set; }
        public int L3Cache { get; set; }
    }

    [Serializable]
    public class ComputerItem
    {
        [Required(ErrorMessage = "Тип обязателен")]
        public string Type { get; set; }

        public Processor Processor { get; set; }

        [Range(4, 65, ErrorMessage = "ОЗУ должно быть от 4")]
        public int RAMSize { get; set; }

        public string RAMType { get; set; }

        public int HDDSize { get; set; }
        public string HDDType { get; set; }

        [MyAttribute(ErrorMessage = "Дата не может быть в будущем")]
        public DateTime PurchaseDate { get; set; }

        [Range(100, 10000, ErrorMessage = "Стоимость должна быть адекватной")]
        public double Cost { get; set; }

        public ComputerItem()
        {
            Processor = new Processor();
        }
    }

    [Serializable]
    public class ComputerLab
    {
        public List<ComputerItem> Computers { get; set; } = new List<ComputerItem>();
    }

    public static class XmlWorker
    {
        public static void Save(ComputerLab lab, string fileName)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ComputerLab));
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                formatter.Serialize(fs, lab);
            }
        }

        public static ComputerLab Load(string fileName)
        {
            if (!File.Exists(fileName))
                return new ComputerLab();

            XmlSerializer formatter = new XmlSerializer(typeof(ComputerLab));
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                return (ComputerLab)formatter.Deserialize(fs);
            }
        }
    }
}