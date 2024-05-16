using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PracticProject3.Cores;

namespace PracticProject3
{
    public partial class PrintSettings : Form
    {
        public PrintSettings()
        {
            InitializeComponent();
        }

        private async void PrintTable(object sender, MouseEventArgs e)
        {
            try
            {
                await DbCore.ConnectLine.OpenAsync();
                await DbCore.DownloadDocumentsList();
                await DbCore.Log("Action", "Скачивание списка документов");
                DbCore.ConnectLine.Close();
                PDFCore obj = new PDFCore("Отчет", "Miks");
                List<int> on_settings = new List<int>();
                on_settings.Add(0);
                if (checkBox1.Checked) { on_settings.Add(1); }
                if (checkBox2.Checked) { on_settings.Add(2); }
                if (checkBox6.Checked) { on_settings.Add(3); }
                if (checkBox3.Checked) { on_settings.Add(4); }
                if (checkBox4.Checked) { on_settings.Add(6); }
                if (checkBox5.Checked) { on_settings.Add(5); }
                obj.Table_Classic(0, DecodeCore.Records, on_settings);
                MessageBox.Show("Отчет создан!", "Операция выполнена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                DbCore.ConnectLine.Close();
                MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
