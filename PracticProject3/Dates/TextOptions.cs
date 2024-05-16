using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;

namespace PracticProject3.Dates
{
    public class TextOptions
    {
        public XFont f_options;
        public XStringFormat s_options;
        public TextOptions()
        {
            f_options = new XFont("Times New Roman", 8, XFontStyle.Regular);
            s_options = new XStringFormat();
            s_options.Alignment = XStringAlignment.Near;
            s_options.LineAlignment = XLineAlignment.Near;
        }
        public void ChangeFont(string font_name, int size, XFontStyle style)
        {
            f_options = new XFont(font_name, size, style);
        }

        public void ChangeFormat(XStringAlignment str, XLineAlignment line)
        {
            s_options = new XStringFormat();
            s_options.Alignment = str;
            s_options.LineAlignment = line;
        }
    }
}
