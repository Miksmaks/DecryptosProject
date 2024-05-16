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
    public partial class AddUser : Form
    {
        public AddUser()
        {
            InitializeComponent();
        }

        private async void AddNewUser(object sender, MouseEventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Отсутствуют данные", "Ошибка при вводе", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                await DbCore.AddUser(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text, Settings.Permissions[comboBox1.SelectedIndex].Id);
                await DbCore.Log("Action", $"Добавлен новый пользователь ({textBox4.Text})");
                MessageBox.Show("Пользователь добавлен!", "Операция выполнена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void PreLoad(object sender, EventArgs e)
        {
            try
            {
                await DbCore.ConnectLine.OpenAsync();
                await DbCore.GetPermissions();
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

        private void OpenList(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            for (int i = 0; i < Settings.Permissions.Count; i++)
            {
                comboBox1.Items.Add($"{Settings.Permissions[i].Name}");
            }
        }
    }
}
