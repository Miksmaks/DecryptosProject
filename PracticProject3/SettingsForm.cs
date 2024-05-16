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
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void AddUser(object sender, MouseEventArgs e)
        {
            AddUser obj = new AddUser();
            obj.ShowDialog();
        }

        private void ChangePerm(object sender, MouseEventArgs e)
        {
            ChangePermissions obj = new ChangePermissions();
            obj.ShowDialog();
        }

    }
}
