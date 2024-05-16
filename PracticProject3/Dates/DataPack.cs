using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticProject3.Dates
{
    public class DataPack
    {
        public string DataPackName = "";
        public List<DataLine> Data = new List<DataLine>();
        public DataPack(string name, List<DataLine> datas)
        {
            DataPackName = name;
            Data = datas;
        }
    }
}
