using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laba2
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public void DisplayComputers(ComputerLab lab)
        {
            if (lab.Computers.Count == 0)
            {
                MessageBox.Show("Нет сохранённых компьютеров.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            StringBuilder message = new StringBuilder();
            message.AppendLine("~~~ Список компьютеров ~~~\n");

            for (int i = 0; i < lab.Computers.Count; i++)
            {
                var comp = lab.Computers[i];
                message.AppendLine($"Компьютер #{i + 1}");
                message.AppendLine($"Тип: {comp.Type}");
                message.AppendLine($"Процессор: {comp.Processor.Manufacturer} {comp.Processor.Model}");
                message.AppendLine($"Ядра: {comp.Processor.Cores}, Частота: {comp.Processor.Frequency} GHz");
                message.AppendLine($"ОЗУ: {comp.RAMSize}GB {comp.RAMType}");
                message.AppendLine($"Диск: {comp.HDDSize}GB {comp.HDDType}");
                message.AppendLine($"Дата покупки: {comp.PurchaseDate.ToShortDateString()}");
                message.AppendLine($"Стоимость: {comp.Cost:F2}$");
                message.AppendLine("──────────────────\n");
            }

            textBox1.Text = message.ToString();
        }

    }
}
