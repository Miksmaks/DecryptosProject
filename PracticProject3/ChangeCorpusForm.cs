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
    public partial class ChangeCorpusForm : Form
    {
        public ChangeCorpusForm()
        {
            InitializeComponent();
        }

        private void OpenList(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            for (int i = 0; i < DecodeCore.Corpuses.Count; i++)
            {
                comboBox1.Items.Add(DecodeCore.Corpuses[i].Name);
            }
        }

        private void Close(object sender, FormClosedEventArgs e)
        {
            DbCore.ConnectLine.Close();
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

        private async void AddCorpus(object sender, MouseEventArgs e)
        {
            if (textBox1.Text != "")
            {
                try
                {
                    await DbCore.AddCorpus(textBox1.Text, textBox2.Text);
                    await DbCore.Log("Action", $"Добавлен новый корпус ({textBox2.Text})");
                    MessageBox.Show("Корпус добавлен", "Операция выполнена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private async void DeleteCorpus(object sender, MouseEventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                try
                {
                    int k = DecodeCore.Corpuses.Find(x => x.Name == (string)comboBox1.SelectedItem).Id;
                    await DbCore.RemoveCorpus(k);
                    await DbCore.Log("Action", $"Удален корпус с id({k})");
                    MessageBox.Show("Корпус удален", "Операция выполнена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
