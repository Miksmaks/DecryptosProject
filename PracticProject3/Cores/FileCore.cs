using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Office.Interop.Word;
using PracticProject3.Dates;

namespace PracticProject3.Cores
{
    public static class FileCore
    {
        static public List<string> BackUpList = new List<string>();
        static public List<string> GetFiles(string path) 
        {
            List<string> list = new List<string>();
            if (!Directory.Exists(path)) { return list; }
            foreach (string str in Directory.GetFileSystemEntries(path)) 
            {
                if (Directory.Exists(str))
                {
                    list.AddRange(GetFiles(str));
                }
                else 
                {
                    if (GetFileType(str) != "docx" && GetFileType(str) != "pdf") { continue; }
                    if (BackUpList.Contains(str)) { continue; }
                    list.Add(str);
                }
            }
            return list;
        }

        static public string GetFileName(string path) 
        {
            return (path.Substring(path.LastIndexOf('\\')+1)).Remove(path.Substring(path.LastIndexOf('\\') + 1).LastIndexOf('.')) ?? "ERROR-FILE";
        }

        static public string GetFileType(string path)
        {
            return path.Substring(path.LastIndexOf('.') + 1) ?? "ERR";
        }

        static public string GetTempPrevious(string path) // Only for docx to pdf
        {
            Application Word = new Application();
            Document doc = Word.Documents.Open(path);
            string str = System.Windows.Forms.Application.StartupPath.ToString() + "\\temp_pdf.pdf";
            doc.ExportAsFixedFormat(str, WdExportFormat.wdExportFormatPDF);
            doc.Close();
            Word.Quit();
            return str;
        }

        static public bool CopyFile(InfoData obj,string path) 
        {
            if (obj.Corpus.Name != null && obj.Type.Name != null && obj.Company.Name != null && obj.Tags != null) 
            {
                FileInfo file = new FileInfo(path);
                string NewPath = $"DocBase\\{obj.Corpus.Name}\\{obj.Type.Name}\\{obj.Company.Name}\\{obj.Tags}";
                if (!Directory.Exists(NewPath)) { Directory.CreateDirectory(NewPath); }
                file.CopyTo(NewPath + $"\\{file.Name}", true);
                return true;
            }
            return false;
        }
    }
}
