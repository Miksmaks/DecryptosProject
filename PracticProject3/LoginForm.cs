using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Wordprocessing;
using PracticProject3.Cores;

namespace PracticProject3
{
    public partial class LoginForm : Form
    {
        string DbName = XMLCode.ReadConfigDbName();
        public LoginForm()
        {
            InitializeComponent();
            //DbCore.CreateConnection("Server=localhost;User Id=Admin;Password=1553;Encrypt=false;");
            /*try
            {
                DbCore.CreateConnection("Server=DESKTOP-5V5KM30\\SQLSERVER2023;Trusted_Connection=True;Encrypt=false;");
            }
            catch (Exception ex)
            {
                DbCore.ConnectLine.Close();
                MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        // TrustServerCertificate=True;

        private async void Profile(object sender, MouseEventArgs e)
        {
            //Clipboard.SetText(Cores.Settings.EncryptString("Пароль"));
            if (textBox1.Text == "" || textBox2.Text == "" || comboBox1.Text == "") 
            {
                MessageBox.Show("Заполните поля", "Пустые поля", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                string SqlServer = comboBox1.Text;
                string Login = textBox1.Text;
                string Password = textBox2.Text;
                string[] profile = XMLCode.ReadConfigProfile();
                if (profile[0] == "" || profile[1] == "") 
                {
                    MessageBox.Show("Не удается прочитать данные в конфиге", "Ошибка конфига", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //DbCore.ConnectionSettings($"{comboBox1.Text}", string _db, string _user, string _password, string _other)
                DbCore.ConnectionSettings(SqlServer, DbName, profile[0], Cores.Settings.DecryptString(profile[1]), "Encrypt=false;");
                DbCore.CreateConnection();
                //MessageBox.Show("0", "Тест", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await DbCore.ConnectLine.OpenAsync();
                await DbCore.BuildDbStruct(DbName); // Для истока: TDMServiceDB
                AuthResponse AuthRequest = await DbCore.Auth(Login,Password);
                if (AuthRequest == AuthResponse.HasProfile)
                {
                    try
                    {
                        await DbCore.Log("System", "Вход в систему");
                        DbCore.ConnectLine.Close();
                    }
                    catch (Exception ex)
                    {
                        DbCore.ConnectLine.Close();
                        MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    XMLCode.AddConfig(SqlServer);
                    RegistrationForm form = new RegistrationForm();
                    form.Show();
                    form.Text = form.Text + $" (Пользователь: {Cores.Settings.SecondName} {Cores.Settings.FirstName[0]}.{Cores.Settings.ThirdName[0]}) Права: {Cores.Settings.Permission}";
                    this.Hide();

                }
                else if (AuthRequest == AuthResponse.NotFound) 
                {
                    DbCore.ConnectLine.Close();
                    MessageBox.Show($"Учетная запись не найдена", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (AuthRequest == AuthResponse.StartUse)
                {
                    AutoRegistration form = new AutoRegistration(1, Login, Password);
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                DbCore.ConnectLine.Close();
                MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadServers(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            List<string> servers = XMLCode.ReadConfigServers();
            for (int i = 0; i < servers.Count; i++) 
            {
                comboBox1.Items.Add(servers[i]);
            }
        }
    }
}
