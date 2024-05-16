using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticProject3.DownloadData
{
    public struct Company
    {
        public int Id;
        public string NameNum;
        public string Name;
        public Company(int id,string name_num,string name) 
        {
            Id = id;
            NameNum = name_num;
            Name = name;
        }
    }
}
