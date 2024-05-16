using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PracticProject3.Dates;
namespace PracticProject3.Cores
{
    enum TitleOpt 
    {
        DocNum,
        DocType,
        NameCompany,
        Corpus,
        RegistrationTime,
        Tags
    }

    public class PDFCore
    {
        PdfDocument Document;
        static List<Title> Titles = new List<Title>() 
        {
            new Title("№ Договора",2), 
            new Title("Тип документации",2),
            new Title("Компания",3),
            new Title("Номер",1),
            new Title("Корпус",1),
            new Title("Дата регистрации",2),
            new Title("Теги",5),
            new Title("",0)
        };

        public string ChooseData(Title str, DownloadData.Record obj) 
        {
            if (str.Name == "№ Договора")
            {
                return obj.DocNum;
            }
            else if (str.Name == "Тип документации")
            {
                return obj.Type;
            }
            else if (str.Name == "Компания")
            {
                return obj.CompanyName;
            }
            else if (str.Name == "Номер")
            {
                return obj.CompanyNum;
            }
            else if (str.Name == "Корпус")
            {
                return obj.Corpus;
            }
            else if (str.Name == "Дата регистрации")
            {
                return obj.RegTime;
            }
            else if (str.Name == "Теги")
            {
                return obj.Tags;
            }
            else 
            {
                return "???";
            }
        }

        public PDFCore(string name, string author)
        {
            Document = new PdfDocument();
            Document.Info.Title = name;
            Document.Info.Author = author;
            Document.AddPage();
        }

        void Rectangle(int Page, XBrush Brush, ESize size)
        {
            XGraphics Board = XGraphics.FromPdfPage(Document.Pages[Page]);
            Board.DrawRectangle(
                Brush,
                new XRect(size.X, size.Y, size.Width, size.Height)
            );
            Board.Dispose();
        }

        void Frame(int Page, XPen Pen, ESize size)
        {
            XGraphics Board = XGraphics.FromPdfPage(Document.Pages[Page]);
            Board.DrawRectangle(
                Pen,
                new XRect(size.X, size.Y, size.Width, size.Height)
            );
            Board.Dispose();
        }

        void Text(int Page, string text, XBrush Brush,TextOptions opt, ESize size)
        {
            XGraphics Board = XGraphics.FromPdfPage(Document.Pages[Page]);
            (new XTextFormatter(Board)).DrawString(
                text,
                opt.f_options,
                Brush,
                new XRect(size.X, size.Y, size.Width, size.Height),
                opt.s_options
            );
            Board.Dispose();
        }

        public void Table_Classic(int Page,List<DownloadData.Record> Data, List<int> opt_title)
        {
            int width = 560; // height = 810
            int X = 20;
            int Y = 20;
            int x_m = 0;
            int N = 0;
            double sum_size = 0.0;
            TextOptions opt = new TextOptions();
            foreach (int obj in opt_title)
            {
                sum_size += Titles[obj].Value;
            }
            List<int> sizes = new List<int>();
            for (int i = 0; i < opt_title.Count; i++)
            {
                sizes.Add((int)(width * (Titles[opt_title[i]].Value / sum_size)));
            }
            while (Data.Count > Page * 39) 
            {
                opt.ChangeFont("Times New Roman", 10, XFontStyle.Bold);
                for (int j = 0; j < sizes.Count; j++)
                {
                    Rectangle(Page, XBrushes.Gray, new ESize(X + x_m, Y, sizes[j], 20));
                    Text(Page, Titles[opt_title[j]].Name, XBrushes.Black, opt, new ESize(X + x_m + 2, Y + 4, sizes[j], 20));
                    Frame(Page, new XPen(XColors.Black), new ESize(X + x_m, Y, sizes[j], 20));
                    x_m += sizes[j];
                }
                x_m = 0;
                opt.ChangeFont("Times New Roman", 8, XFontStyle.Regular);
                if (Data.Count <= (Page + 1) * 39) 
                {
                    N = Data.Count - Page * 39 + 1; 
                }
                else 
                {
                    N = 40;
                }
                for (int i = 1; i < N; i++)
                {
                    for (int j = 0; j < sizes.Count; j++)
                    {
                        Text(Page, ChooseData(Titles[opt_title[j]], Data[Page * 39 + i - 1]), XBrushes.Black, opt, new ESize(X + x_m + 2, Y + 20 * i + 5, sizes[j], 20));
                        Frame(Page, new XPen(XColors.Black), new ESize(X + x_m, Y + 20 * i, sizes[j], 20));
                        x_m += sizes[j];
                    }
                    x_m = 0;
                }
                if (N == 40) 
                {
                    Document.AddPage();
                }
                Page++;
            }
            Document.Save($"Reports\\Report {DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year} ({DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}).pdf");
        }
    }
}
