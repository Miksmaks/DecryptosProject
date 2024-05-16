using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticProject3.DownloadData;

namespace PracticProject3.Dates
{
    public struct InfoData
    {
        // Файловая инфа
        public string DocNum;
        public DocType Type; // добавляется коррекция
        public Company Company;
        public Corpus Corpus;
        public string RegTime;
        public string Tags;
        // Системная инфа
        public string SysPath;
        public string SysType;
        public string SysHash;
    }
}
