using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf.interfaces;
using iTextSharp.text.io;

namespace BetaSign
{
    public delegate void saveFileInfo(Dictionary<string,string> info);

    public partial class FileInfo : Form
    {
        public PdfReader reader;
        public string filePath;
        public saveFileInfo saveFileInfoFn;

        public FileInfo()
        {
            InitializeComponent();
        }

        public void loadFileInfo()
        {
            txtFile.Text = filePath;

            Dictionary<string, string> info = reader.Info;
            if (info.ContainsKey("Title")) txtTitle.Text = info["Title"];
            if (info.ContainsKey("Subject")) txtSubject.Text = info["Subject"];
            if (info.ContainsKey("Author")) txtAuthor.Text = info["Author"];
            if (info.ContainsKey("Keywords")) txtKeywords.Text = info["Keywords"];
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> info = new Dictionary<string, string>();
            info["Title"] = txtTitle.Text;
            info["Subject"] = txtSubject.Text;
            info["Author"] = txtAuthor.Text;
            info["Keywords"] = txtKeywords.Text;

            saveFileInfoFn(info);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FileInfo_Load(object sender, EventArgs e)
        {
            loadFileInfo();
        }
    }
}
