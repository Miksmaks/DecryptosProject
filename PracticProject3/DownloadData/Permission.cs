using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticProject3.DownloadData
{
    public struct Permission
    {
        public int Id;
        public string Name;
        public Permission(int id,string name) 
        {
            Id = id;
            Name = name;
        }
    }
}
