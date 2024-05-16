using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticProject3.DownloadData;
using PracticProject3.Dates;
using System.Security.Cryptography;
using System.IO;

namespace PracticProject3.Cores
{
    public static class DecodeCore
    {
        static public List<Company> Companies = new List<Company>();
        static public List<Corpus> Corpuses = new List<Corpus>();
        static public List<DocType> DocTypes = new List<DocType>();
        static public List<Record> Records = new List<Record>();

        static public void ClearData() 
        {
            Companies.Clear();
            Corpuses.Clear();
            DocTypes.Clear();
        }

        static public List<InfoData> DecodeNames(List<string> Names) 
        {
            List<InfoData> decode_list = new List<InfoData>();
            for (int i = 0; i < Names.Count; i++) 
            {
                decode_list.Add(Decode(Names[i]));
            }
            return decode_list;
        }

        static private List<string> GetSplit(string name) 
        {
            List<string> Arr = new List<string>();
            string[] mas = name.Split('-');
            if (mas.Length > 2) 
            {
                Arr.Add(mas[0]+"-"+mas[1]);
                for (int i = 2; i < mas.Length; i++)
                {
                    Arr.Add(mas[i]);
                }
                return Arr;
            }
            return Arr;
        }

        static public InfoData Decode(string name) 
        {
            List<string> DisList = GetSplit(FileCore.GetFileName(name));
            if (DisList.Count != 5) { return new InfoData(); }
            InfoData data = new InfoData();
            Company obj1 = Companies.Find(x => x.NameNum == DisList[1]);
            if (obj1.Id != default) { data.Company = obj1; }
            DocType obj2 = DocTypes.Find(x => x.Name == DisList[2]);
            if (obj2.Id != default) { data.Type = obj2; }
            Corpus obj3 = Corpuses.Find(x => x.NumName == DisList[3]);
            if (obj3.Id != default) { data.Corpus = obj3; }
            if (data.Company.Id == default || data.Type.Id == default || data.Corpus.Id == default) { return new InfoData(); }
            data.DocNum = DisList[0];
            data.Tags = DisList[4];
            data.RegTime = DateTime.Now.ToString();
            data.SysPath = System.Windows.Forms.Application.StartupPath.ToString() + $"\\DocBase\\" +
                $"{data.Corpus.Name}\\{data.Type.Name}\\{data.Company.Name}\\{data.Tags}\\{FileCore.GetFileName(name)}.{FileCore.GetFileType(name)}";
            FileStream fileStream = (new FileInfo(name)).Open(FileMode.Open);
            fileStream.Position = 0;
            byte[] hash = (SHA256.Create()).ComputeHash(fileStream);
            fileStream.Close();
            data.SysHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            data.SysType = FileCore.GetFileType(name);
            return data;
        }
    }
}
