using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticProject3.DownloadData
{
    public struct Corpus
    {
        public int Id;
        public string NumName;
        public string Name;
        public Corpus(int id, string num_name, string name)
        {
            Id = id;
            NumName = num_name;
            Name = name;
        }
    }
}
