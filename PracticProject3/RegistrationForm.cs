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
using PracticProject3.Dates;

using ClosedXML.Excel;
//using DocumentFormat.OpenXml;

namespace PracticProject3
{
    public partial class RegistrationForm : Form
    {
        public RegistrationForm()
        {
            InitializeComponent();
            //DbCore.ConnectionSettings("localhost", "DecryptosBase", "Admin", "1553", "False");
            //DbCore.CreateConnection();
        }


        private void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = listView1.Columns[e.ColumnIndex].Width;
        }


        private void AddFiles(object sender, MouseEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                listView1.Items.Clear();
                FileCore.BackUpList.AddRange(FileCore.GetFiles(folder.SelectedPath));
                foreach (string str in FileCore.BackUpList)
                {
                    listView1.Items.Add(FileCore.GetFileName(str)).SubItems.AddRange(
                        new ListViewItem.ListViewSubItem[]{
                            new ListViewItem.ListViewSubItem(listView1.Items[listView1.Items.Count-1],FileCore.GetFileType(str)),
                            new ListViewItem.ListViewSubItem(listView1.Items[listView1.Items.Count-1],"*")
                        }
                    );
                }
            }
        }

        private void ClearList(object sender, MouseEventArgs e)
        {
            FileCore.BackUpList.Clear();
            listView1.Items.Clear();
        }

        private void Previous(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems[0] != null)
            {
                string path = FileCore.BackUpList[listView1.SelectedIndices[0]];
                if (FileCore.GetFileType(path) == "docx")
                {
                    webView21.Source = new Uri(FileCore.GetTempPrevious(path));
                }
                else if (FileCore.GetFileType(path) == "pdf")
                {
                    webView21.Source = new Uri(path);
                }
                else
                {
                    MessageBox.Show("Этот файл не имеет расширения docx или pdf", "Недоступно", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OpenGuide(object sender, EventArgs e)
        {
            GuideForm obj = new GuideForm();
            //await obj.RefreshList();
            obj.Show();
        }

        private async void Registration(object sender, MouseEventArgs e)
        {
            if (FileCore.BackUpList.Count != 0)
            {
                try
                {
                    await DbCore.ConnectLine.OpenAsync();
                    await DbCore.DownloadData();
                    List<Dates.InfoData> check_list = DecodeCore.DecodeNames(FileCore.BackUpList);
                    List<Dates.InfoData> reg_list = new List<Dates.InfoData>();
                    //string errors = "";
                    for (int i = 0; i < check_list.Count; i++)
                    {
                        if (FileCore.CopyFile(check_list[i], FileCore.BackUpList[i]))
                        {
                            reg_list.Add(check_list[i]);
                            listView1.Items[i].SubItems[2].Text = "?";
                        }
                        else
                        {
                            listView1.Items[i].SubItems[2].Text = "✘";
                            //errors += $"{FileCore.GetFileName(FileCore.BackUpList[i])}\n";
                        }
                    }
                    await DbCore.SendData(Settings.UserId, reg_list);
                    await DbCore.Log("Action", $"Зарегистрировано {check_list.Count - reg_list.Count} документов");
                    DbCore.ConnectLine.Close();
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        if (listView1.Items[i].SubItems[2].Text == "?") 
                        {
                            listView1.Items[i].SubItems[2].Text = "✔";
                        }
                    }
                    MessageBox.Show("Документы были добавлены!", "Операция выполнена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    /*if (errors != "")
                    {
                        MessageBox.Show("Следующие документы не прошли проверку:\n" + errors, "Незарегистрированные документы", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }*/
                }
                catch (Exception ex)
                {
                    DbCore.ConnectLine.Close();
                    MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Список для регистрации пуст", "Пустой список", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WriteRecord(object sender, MouseEventArgs e)
        {
            PrintSettings obj = new PrintSettings();
            obj.ShowDialog();
        }

        private async void RegistrationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                await DbCore.ConnectLine.OpenAsync();
                await DbCore.Log("System", "Выход из системы");
                DbCore.ConnectLine.Close();
                Application.Exit();
            }
            catch (Exception ex)
            {
                DbCore.ConnectLine.Close();
                MessageBox.Show($"Источник: {ex.Source}\nСообщение: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenSettings(object sender, EventArgs e)
        {
            if (Settings.Permission == "Owner")
            {
                SettingsForm obj = new SettingsForm();
                //await obj.RefreshList();
                obj.Show();
            }
            else
            {
                MessageBox.Show("Недостаточно прав!", "Ошибка прав", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExcelToSql(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Excel file (*.xlsx)|*.xlsx";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
            string filePath = openFileDialog.FileName;

            List<DataPack> dataPacks = new List<DataPack>();
            XLWorkbook Book = new XLWorkbook(filePath);
            for (int k = 1; k <= Book.Worksheets.Count; k++)
            {
                List<DataLine> dataLines = new List<DataLine>()
                {
                    new DataLine()
                    {
                        line = Book.Worksheet(k)
                        .FirstRow()
                        .Cells()
                        .Select(x => x.Value.ToString().Replace(" ","_").Trim())
                        .ToList()
                    }
                };
                var rows = Book.Worksheet(k).Rows().Skip(1);
                foreach (IXLRow row in rows) 
                {
                    
                    if (row.Cells(false).Count() < dataLines.First().line.Count) 
                    {
                        continue;
                    }
                    /*
                    if (row.Cells().All(x => string.IsNullOrEmpty(x.Value.ToString()))) 
                    {
                        continue;
                    }*/
                    dataLines.Add(
                        new DataLine()
                        {
                            line = row.Cells(false)
                            .Select(x => x.Value.ToString())
                            .ToList()
                        }
                    ); 
                }
                dataPacks.Add(new DataPack(Book.Worksheet(k).Name, dataLines));
            }
            DbCore.CreateDbDataTable(dataPacks);
            /*
            Excel.Application ex = new Excel.Application();
            Excel.Workbook Book = ex.Workbooks.Open(filePath);
            Excel.Worksheet WorkSheet;
            List<DataPack> dataPacks = new List<DataPack>();
            for (int k = 0; k < Book.Sheets.Count; k++)
            {
                WorkSheet = (Excel.Worksheet)ex.Worksheets[k + 1];
                List<DataLine> dataLines = new List<DataLine>();
                int i = 0, j = 0;
                DataLine start_obj = new DataLine();
                while (WorkSheet.Cells[i + 1, j + 1].Value2 != null)
                {
                    start_obj.line.Add(WorkSheet.Cells[i + 1, j + 1].Value2.ToString().Replace(" ", "_"));
                    j++;
                }
                dataLines.Add(start_obj);
                i++;
                j = 0;
                while (WorkSheet.Cells[i + 1, 1].Value2 != null)
                {
                    DataLine obj = new DataLine();
                    while (j < start_obj.line.Count)
                    {
                        if (WorkSheet.Cells[i + 1, j + 1].Value2 == null)
                        {
                            obj.line.Add("");
                        }
                        else
                        {
                            obj.line.Add(WorkSheet.Cells[i + 1, j + 1].Value2.ToString());
                        }
                        j++;
                    }
                    dataLines.Add(obj);
                    i++;
                    j = 0;
                }

                dataPacks.Add(new DataPack(WorkSheet.Name, dataLines));
            }*/
            //XMLCode.CreateXMLDataPack(dataPacks);
            // localhost\\SQLSERVER2023 test
            //DbCode DbObj = new DbCode(XMLCode.GetSettingsConfig());

            //DbCore.CreateDbDataTable(dataPacks);
        }

        private void PreLoad(object sender, EventArgs e)
        {
            if (Settings.Permission != "Owner")
            {
                menuStrip1.Items[2].Visible = false;
            }
        }
    }
}
