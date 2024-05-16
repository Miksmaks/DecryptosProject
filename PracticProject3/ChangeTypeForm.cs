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
    public partial class ChangeTypeForm : Form
    {
        public ChangeTypeForm()
        {
            InitializeComponent();
        }

        private async void PreLoad(object sender, EventArgs e)
        {
            try
            {
                await DbCore.ConnectLine.OpenAsync();
                await DbCore.DownloadData();
            }
            catch (Exception ex)
            {
                DbCore.ConnectLine.Close();
                MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Close(object sender, FormClosedEventArgs e)
        {
            DbCore.ConnectLine.Close();
        }

        private void LoadList(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            for (int i = 0; i < DecodeCore.DocTypes.Count; i++)
            {
                comboBox1.Items.Add(DecodeCore.DocTypes[i].FullName);
            }
        }

        private async void AddType(object sender, MouseEventArgs e)
        {
            if (textBox1.Text != "")
            {
                try
                {
                    await DbCore.AddDocType(textBox2.Text, textBox1.Text);
                    await DbCore.Log("Action", $"Тип документации ({textBox2.Text}) был добавлен");
                    MessageBox.Show("Тип документации добавлен", "Операция выполнена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Отсутствуют данные", "Ошибка при вводе", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DeleteType(object sender, MouseEventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                try
                {
                    int k = DecodeCore.DocTypes.Find(x => x.FullName == (string)comboBox1.SelectedItem).Id;
                    await DbCore.RemoveDocType(k);
                    await DbCore.Log("Action", $"Тип документации с id({k}) был удален");
                    MessageBox.Show("Тип документации удален", "Операция выполнена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Отсутствуют данные", "Ошибка при вводе", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
