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
    public partial class ChangePermissions : Form
    {
        public ChangePermissions()
        {
            InitializeComponent();
        }

        private async void AddPerm(object sender, MouseEventArgs e)
        {
            if (comboBox1.SelectedIndex == -1  || comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Отсутствуют данные", "Ошибка при вводе", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                int k1 = Settings.Users[comboBox1.SelectedIndex].Id;
                DownloadData.Permission k2 = Settings.Permissions[comboBox2.SelectedIndex];
                await DbCore.ChangePermissions(k1, k2.Id);
                await DbCore.Log("Action", $"Права пользователя с id({k1}) были изменены на {k2.Name}");
                MessageBox.Show("Права изменены!", "Операция выполнена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DeleteUser(object sender, MouseEventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Отсутствуют данные", "Ошибка при вводе", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                int k = Settings.Users[comboBox1.SelectedIndex].Id;
                await DbCore.DeleteUser(k);
                await DbCore.Log("Action", $"Пользователь с id({k}) был удален");
                comboBox1.Items.Clear();
                await DbCore.GetUsers();
                MessageBox.Show("Пользователь удален!", "Операция выполнена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenUserList(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            for (int i = 0; i < Settings.Users.Count; i++)
            {
                comboBox1.Items.Add($"{Settings.Users[i].SecondName} {Settings.Users[i].FirstName[0]}.{Settings.Users[i].ThirdName[0]}");
            }
        }

        private void OpenPermList(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            for (int i = 0; i < Settings.Permissions.Count; i++)
            {
                comboBox2.Items.Add($"{Settings.Permissions[i].Name}");
            }
        }

        private async void PreLoad(object sender, EventArgs e)
        {
            try
            {
                await DbCore.ConnectLine.OpenAsync();
                await DbCore.GetPermissions();
                await DbCore.GetUsers();
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
    }
}
