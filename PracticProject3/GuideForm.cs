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
using PracticProject3.DownloadData;

namespace PracticProject3
{
    public partial class GuideForm : Form
    {
        public GuideForm()
        {
            InitializeComponent();
        }

        public async Task RefreshList() 
        {
            try
            {
                treeView1.Nodes.Clear();
                await DbCore.ConnectLine.OpenAsync();
                await DbCore.DownloadData();
                await DbCore.DownloadDocumentsList();
                DbCore.ConnectLine.Close();
                TreeNode k = new TreeNode("Строительная документация");
                k.Toggle();
                for (int i = 0; i < DecodeCore.Corpuses.Count; i++)
                {
                    FillNode(k, i, 0);
                }
                treeView1.Nodes.Add(k);
            }
            catch (Exception ex)
            {
                DbCore.ConnectLine.Close();
                MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FillNode(TreeNode obj, int n, int lvl) 
        {
            TreeNode L = new TreeNode();
            L.Toggle();
            obj.Nodes.Add(L);
            if (lvl == 0)
            {
                L.Text = DecodeCore.Corpuses[n].Name;
                for (int i = 0; i < DecodeCore.DocTypes.Count; i++)
                {
                    FillNode(L, i,1);
                }
            }
            else if (lvl == 1)
            {
                L.Text = DecodeCore.DocTypes[n].FullName;
                for (int i = 0; i < DecodeCore.Companies.Count; i++)
                {
                    FillNode(L, i, 2);
                }
            }
            else
            {
                L.Text = DecodeCore.Companies[n].Name;
                for (int i = 0; i < DecodeCore.Records.Count; i++)
                {
                    //MessageBox.Show($"{DecodeCore.Records[i].CompanyName} {DecodeCore.Records[i].TypeName} {DecodeCore.Records[i].Corpus}");
                    if (L.Text == DecodeCore.Records[i].CompanyName && L.Parent.Text == DecodeCore.Records[i].TypeName && L.Parent.Parent.Text == DecodeCore.Records[i].Corpus) 
                    {
                        L.Nodes.Add(new TreeNode(DecodeCore.Records[i].DocNum));
                    }
                }
            }
        }

        private void ChooseDoc(object sender, MouseEventArgs e)
        {
            if (treeView1.SelectedNode != null) 
            {
                bool k = true;
                for (int i = 0; i < DecodeCore.Records.Count; i++) 
                {
                    if (treeView1.SelectedNode.Text == DecodeCore.Records[i].DocNum) 
                    {
                        k = false;
                        textBox1.Text = DecodeCore.Records[i].DocNum;
                        textBox2.Text = DecodeCore.Records[i].CompanyNum;
                        textBox3.Text = DecodeCore.Records[i].CompanyName;
                        textBox4.Text = DecodeCore.Records[i].Type;
                        textBox5.Text = DecodeCore.Records[i].CorpusNum;
                        textBox6.Text = DecodeCore.Records[i].Tags;
                        textBox7.Text = DecodeCore.Records[i].RegTime;
                        textBox8.Text = DecodeCore.Records[i].TypeName;
                        textBox9.Text = DecodeCore.Records[i].Corpus;
                        break;
                    }
                }
                if (k) 
                {
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                    textBox6.Clear();
                    textBox7.Clear();
                    textBox8.Clear();
                    textBox9.Clear();
                }
            }
        }

        private async void PreLoad(object sender, EventArgs e)
        {
            if (Settings.Permission != "Admin" && Settings.Permission != "Owner")
            {
                for (int i = 0; i < menuStrip2.Items.Count; i++)
                {
                    menuStrip2.Items[i].Visible = false;
                }
            }
            await RefreshList();
        }

        private async void CompanyForm(object sender, EventArgs e)
        {
            if (Settings.Permission == "Admin" || Settings.Permission == "Owner")
            {
                ChangeCompanyForm form = new ChangeCompanyForm();
                form.ShowDialog();
                await RefreshList();
            }
            else 
            {
                MessageBox.Show("Недостаточно прав!", "Ошибка прав", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void TypeForm(object sender, EventArgs e)
        {
            if (Settings.Permission == "Admin" || Settings.Permission == "Owner")
            {
                ChangeTypeForm form = new ChangeTypeForm();
                form.ShowDialog();
                await RefreshList();
            }
            else
            {
                MessageBox.Show("Недостаточно прав!", "Ошибка прав", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CorpusForm(object sender, EventArgs e)
        {
            if (Settings.Permission == "Admin" || Settings.Permission == "Owner")
            {
                ChangeCorpusForm form = new ChangeCorpusForm();
                form.ShowDialog();
                await RefreshList();
            }
            else
            {
                MessageBox.Show("Недостаточно прав!", "Ошибка прав", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
