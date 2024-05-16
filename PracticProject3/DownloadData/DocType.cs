using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticProject3.DownloadData
{
    public struct DocType
    {
        public int Id;
        public string FullName;
        public string Name;
        public DocType(int id, string full_name, string name)
        {
            Id = id;
            FullName = full_name;
            Name = name;
        }
    }
}
