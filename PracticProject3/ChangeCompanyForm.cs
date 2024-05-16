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
    public partial class ChangeCompanyForm : Form
    {
        public ChangeCompanyForm()
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

        private void CloseForm(object sender, FormClosedEventArgs e)
        {
            DbCore.ConnectLine.Close();
        }

        private void OpenList(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            for (int i = 0; i < DecodeCore.Companies.Count; i++) 
            {
                comboBox1.Items.Add(DecodeCore.Companies[i].Name);
            }
        }

        private async void AddCompany(object sender, MouseEventArgs e)
        {
            if (textBox2.Text != "" && textBox1.Text != "")
            {
                try
                {
                    await DbCore.AddCompany(textBox2.Text, textBox1.Text);
                    await DbCore.Log("Action", $"Добавлена новая компания ({textBox1.Text})");
                    MessageBox.Show("Компания добавлена", "Операция выполнена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private async void DeleteCompany(object sender, MouseEventArgs e)
        {
            if (comboBox1.SelectedItem != null) 
            {
                try
                {
                    int k = DecodeCore.Companies.Find(x => x.Name == (string)comboBox1.SelectedItem).Id;
                    await DbCore.RemoveCompany(k);
                    await DbCore.Log("Action", $"Удалена компания с id({k})");
                    MessageBox.Show("Компания удалена", "Операция выполнена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
