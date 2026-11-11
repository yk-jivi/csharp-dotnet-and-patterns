using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Laba2
{
    public partial class Computer : Form
    {

        private ComputerLab lab = new ComputerLab();
        private string fileName = "computers.xml";
        private int currentIndex = 0;
        private Timer timer = new Timer();
        private bool isDocked = true;

        public Computer()
        {
            InitializeComponent();

            trackBarRamSize.Scroll += TrackBarRamSize_Scroll;
            trackBarHddSdd.Scroll += TrackBarHddSdd_Scroll;
            trackBarCore.Scroll += TrackBarCore_Scroll;

            radioButtonDDR4.Checked = true;
            UpdateTextValues();

            LoadData();

            timer.Interval = 1000;
            timer.Tick += (s, e) =>
            {
                toolStripStatusLabelTime.Text = DateTime.Now.ToString();
            };
            timer.Start();
        }

        private void UpdateStatus(string action)
        {
            lab = XmlWorker.Load(fileName);
            toolStripStatusLabelCount.Text = "Объектов: " + lab.Computers.Count;
            toolStripStatusLabelAction.Text = "Последнее действие: " + action;
        }

        private void ShowComputer(int index)
        {
            if (lab.Computers.Count == 0) return;

            var comp = lab.Computers[index];

            string message =
                $"Компьютер #{index + 1}\n\n" +
                $"Тип: {comp.Type}\n" +
                $"Процессор: {comp.Processor.Manufacturer} {comp.Processor.Model}\n" +
                $"ОЗУ: {comp.RAMSize}GB {comp.RAMType}\n" +
                $"Диск: {comp.HDDSize}GB {comp.HDDType}\n" +
                $"Дата: {comp.PurchaseDate.ToShortDateString()}\n" +
                $"Стоимость: {comp.Cost:F2}$";

            MessageBox.Show(message);
        }

        private void UpdateTextValues()
        {
            textBoxRamValue.Text = trackBarRamSize.Value + " ГБ";
            textBoxHddSdd.Text = trackBarHddSdd.Value + " ГБ";
            textBoxCore.Text = trackBarCore.Value + " ядер";
        }

        private void TrackBarRamSize_Scroll(object sender, EventArgs e)
        {
            textBoxRamValue.Text = trackBarRamSize.Value + " ГБ";
        }

        private void TrackBarHddSdd_Scroll(object sender, EventArgs e)
        {
            textBoxHddSdd.Text = trackBarHddSdd.Value + " ГБ";
        }

        private void TrackBarCore_Scroll(object sender, EventArgs e)
        {
            textBoxCore.Text = trackBarCore.Value + " ядер";
        }

        private void LoadData()
        {
            try
            {
                lab = XmlWorker.Load(fileName);
            }
            catch
            {
                lab = new ComputerLab();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                double computerCost = CalculateComputerCost();
                int count = (int)numericUpDown1.Value;

                MessageBox.Show(
                    $"Стоимость одного компьютера: {computerCost:F2} $\n" +
                    $"Стоимость {count} компьютеров: {computerCost * count:F2} $",
                    "Расчет стоимости",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }


        private double CalculateComputerCost()
        {
            double cost = 200;

            if (comboBox1.Text == "Сервер")
                cost += 500;
            else if (comboBox1.Text == "Рабочая станция")
                cost += 300;
            else if (comboBox1.Text == "Ноутбук")
                cost += 150;

            cost += trackBarCore.Value * 20;
            cost += (double)numericUpDownFrequency.Value * 30;
            cost += (int)numericUpDownCacheL3.Value * 0.5;

            cost += trackBarRamSize.Value * 10;

            cost += trackBarHddSdd.Value * 0.2;
            if (TypeDisk.Text == "SSD") cost += 100;

            return cost;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                lab = XmlWorker.Load(fileName);

                if (comboBoxProd.Text == "" || comboBoxModel.Text == "")
                {
                    MessageBox.Show("Заполните данные процессора.", "Ошибка");
                    return;
                }

                int countToAdd = (int)numericUpDown1.Value;
                double computerCost = CalculateComputerCost();

                for (int i = 0; i < countToAdd; i++)
                {
                    ComputerItem newComputer = new ComputerItem();

                    newComputer.Type = comboBox1.Text;

                    newComputer.Processor.Manufacturer = comboBoxProd.Text;
                    newComputer.Processor.Series = comboBoxSeria.Text;
                    newComputer.Processor.Model = comboBoxModel.Text;
                    newComputer.Processor.Cores = trackBarCore.Value;
                    newComputer.Processor.Frequency = (double)numericUpDownFrequency.Value;
                    newComputer.Processor.MaxFrequency = (double)numericUpDownMaxFrequency.Value;
                    newComputer.Processor.Architecture = (int)numericUpDownArchitecture.Value;
                    newComputer.Processor.L1Cache = (int)numericUpDownCacheL1.Value;
                    newComputer.Processor.L2Cache = (int)numericUpDownCacheL2.Value;
                    newComputer.Processor.L3Cache = (int)numericUpDownCacheL3.Value;

                    newComputer.RAMSize = trackBarRamSize.Value;

                    if (radioButtonDDR43.Checked)
                        newComputer.RAMType = "DDR3";
                    else if (radioButtonDDR4.Checked)
                        newComputer.RAMType = "DDR4";
                    else if (radioButtonDDR5.Checked)
                        newComputer.RAMType = "DDR5";

                    newComputer.HDDSize = trackBarHddSdd.Value;
                    newComputer.HDDType = TypeDisk.Text;

                    newComputer.PurchaseDate = dateTimePicker1.Value;

                    newComputer.Cost = computerCost;

                    var context = new ValidationContext(newComputer);
                    var results = new List<ValidationResult>();

                    bool isValid = Validator.TryValidateObject(newComputer, context, results, true);

                    if (!isValid)
                    {
                        string errors = "";

                        foreach (var error in results)
                        {
                            errors += error.ErrorMessage + "\n";
                        }

                        MessageBox.Show(errors, "Ошибка валидации");
                        return;
                    }

                    lab.Computers.Add(newComputer);
                }

                XmlWorker.Save(lab, fileName);

                MessageBox.Show(
                    $"Добавлено компьютеров: {countToAdd}\n" +
                    $"Стоимость одного: {computerCost:F2} $\n" +
                    $"Общая стоимость: {(computerCost * countToAdd):F2} $",
                    "Успешно сохранено",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            try
            {

                lab = XmlWorker.Load(fileName);

                if (lab.Computers.Count == 0)
                {
                    MessageBox.Show("Нет сохраненных компьютеров.", "Информация");
                    return;
                }


                string message = "~~~ Список ~~~\n\n";

                for (int i = 0; i < lab.Computers.Count; i++)
                {
                    var comp = lab.Computers[i];
                    message += $"Компьютер #{i + 1}\n";
                    message += $"Тип: {comp.Type}\n";
                    message += $"Процессор: {comp.Processor.Manufacturer} {comp.Processor.Model}\n";
                    message += $"ОЗУ: {comp.RAMSize}GB {comp.RAMType}\n";
                    message += $"Диск: {comp.HDDSize}GB {comp.HDDType}\n";
                    message += $"Дата: {comp.PurchaseDate.ToShortDateString()}\n";
                    message += $"Стоимость: {comp.Cost:F2}$\n";
                    message += "──────────────────\n\n";
                }

                message += $"Всего компьютеров: {lab.Computers.Count}";

                MessageBox.Show(message, "Список компьютеров", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                lab = XmlWorker.Load(fileName);

                Form2 listForm = new Form2();

                listForm.DisplayComputers(lab);

                listForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void поМоделиПроцессораToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lab = XmlWorker.Load(fileName);

            string model = Microsoft.VisualBasic.Interaction.InputBox("Введите модель:");

            var result = lab.Computers
                .Where(c => c.Processor.Model == model)
                .ToList();

            ShowResults(result);
        }
        private void ShowResults(List<ComputerItem> list)
        {
            string message = "";

            foreach (var comp in list)
            {
                message += comp.Type + " | ";
                message += comp.Processor.Model + " | ";
                message += comp.Cost + "$\n";
                message += $"Дата покупки: {comp.PurchaseDate.ToShortDateString()}\n";
            }

            MessageBox.Show(message);
        }

        private void поРегулярномуВыражениюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lab = XmlWorker.Load(fileName);

            string pattern = Microsoft.VisualBasic.Interaction.InputBox("Введите regex:");

            Regex regex = new Regex(pattern);

            var result = lab.Computers
                .Where(c => regex.IsMatch(c.Processor.Model))
                .ToList();

            ShowResults(result);
        }

        private void поСтоимостиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lab = XmlWorker.Load(fileName);

            var sorted = lab.Computers
                .OrderBy(c => c.Cost)
                .ToList();

            ShowResults(sorted);
        }

        private void поДатеПокупкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lab = XmlWorker.Load(fileName);

            var sorted = lab.Computers
                .OrderBy(c => c.PurchaseDate)
                .ToList();

            ShowResults(sorted);
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lab = XmlWorker.Load(fileName);

            string json = JsonSerializer.Serialize(lab);

            File.WriteAllText("result.json", json);

            MessageBox.Show("Сохранено в result.json");
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
            "Лабораторная работа №3\nВерсия 1.0\nРазработчик: Шматова Юлия Сергеевна");
        }

        //Поиск
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            поРегулярномуВыражениюToolStripMenuItem_Click(sender, e);
            UpdateStatus("Поиск выполнен");
        }

        //Сортировка
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            поСтоимостиToolStripMenuItem_Click(sender, e);
            UpdateStatus("Сортировка по стоимости");
        }

        //Очистить
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            lab = XmlWorker.Load(fileName);

            lab.Computers.Clear();
            XmlWorker.Save(lab, fileName);

            MessageBox.Show("Список очищен");
            UpdateStatus("Очистка списка");
        }

        //Удалить
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            lab = XmlWorker.Load(fileName);

            if (lab.Computers.Count > 0)
            {
                lab.Computers.RemoveAt(lab.Computers.Count - 1);
                XmlWorker.Save(lab, fileName);

                MessageBox.Show("Удалён последний компьютер");
                UpdateStatus("Удаление объекта");
            }
            else
            {
                MessageBox.Show("Список пуст");
            }
        }

        //Назад
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            lab = XmlWorker.Load(fileName);

            if (currentIndex > 0)
            {
                currentIndex--;
                ShowComputer(currentIndex);
                UpdateStatus("Назад");
            }
        }

        //Вперёд
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            lab = XmlWorker.Load(fileName);

            if (currentIndex < lab.Computers.Count - 1)
            {
                currentIndex++;
                ShowComputer(currentIndex);
                UpdateStatus("Вперёд");
            }
        }
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (isDocked)
            {
                toolStrip1.Dock = DockStyle.None;
                toolStrip1.Location = new System.Drawing.Point(50, 50);
                toolStrip1.GripStyle = ToolStripGripStyle.Visible;
                toolStrip1.AllowItemReorder = true;
                toolStripButton8.Checked = false;
                isDocked = false;
            }
            else
            {
                toolStrip1.Dock = DockStyle.Top;
                toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
                toolStrip1.AllowItemReorder = false;
                toolStripButton8.Checked = true;
                isDocked = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            toolStrip1.Visible = !toolStrip1.Visible;
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}