using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticProject3.DownloadData
{
    public struct Record
    {
        public string DocNum;
        public string Type; 
        public string TypeName;
        public string CompanyName;
        public string CompanyNum;
        public string Corpus;
        public string CorpusNum;
        public string RegTime;
        public string Tags;
        public Record(
            string docnum, string type,string type_name, 
            string company_name, string company_num, string corpus, 
            string corpus_num, string regtime, string tags) 
        {
            DocNum = docnum;
            Type = type; 
            TypeName = type_name;
            CompanyName = company_name;
            CompanyNum = company_num;
            Corpus = corpus;
            CorpusNum = corpus_num;
            RegTime = regtime;
            Tags = tags;
         }
    }
}
