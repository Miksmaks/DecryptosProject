using PracticProject3.Cores;
using PracticProject3.Dates;
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
    public partial class ProgressList : Form
    {
        public ProgressList(int count)
        {
            InitializeComponent();
            progressBar1.Maximum = count;
        }

        public void AddProgress()
        {
            progressBar1.Value++;
            label1.Text = $"Загружено {progressBar1.Value} из {progressBar1.Maximum}";
        }

        public void AddList(List<string> Tables, List<string> Ids, List<string> Errors)
        {
            progressBar1.Value = progressBar1.Maximum;
            for (int i = 0; i < Tables.Count; i++)
            {
                listView1.Items.Add(Tables[i]);
                listView1.Items[i].SubItems.Add(Ids[i]);
                listView1.Items[i].SubItems.Add(Errors[i]);
            }
        }
    }
}
