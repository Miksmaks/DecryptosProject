using PracticProject3.Cores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PracticProject3
{
    public partial class AutoRegistration : Form
    {
        int Permission;
        string Login;
        string Password;
        public AutoRegistration(int PermissionId, string login,string password)
        {
            InitializeComponent();
            Permission = PermissionId;
            Login = login;
            Password = password;
            if (PermissionId == 1)
            {
                MessageBox.Show("Вам выданы полные права администратора", "Проверка списка пользователей", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void Registration(object sender, MouseEventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "") 
            {
                MessageBox.Show("Не все поля заполнены", "Ошибка заполнения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }
            try
            {
                await DbCore.AddUser(textBox1.Text, textBox2.Text, textBox3.Text, Login,Password, Permission);
                await DbCore.Log("System", "Автоматическая регистрация нового пользователя");
                MessageBox.Show("Пользователь зарегистрирован!", "Операция выполнена", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DbCore.ConnectLine.Close();
            }
            catch (Exception ex)
            {
                DbCore.ConnectLine.Close();
                MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Close();
        }
    }
}
