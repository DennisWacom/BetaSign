using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf.interfaces;
using iTextSharp.text.io;
using iTextSharp.text.pdf.security;
using Florentis;
using PdfiumViewer;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace BetaSign
{
    public partial class Form1 : Form
    {
        public PdfReader reader;
        public PdfStamper stamper;
        public string defaultPdf = "Lorem Ipsum.pdf";   
        public string originalFile = "";
        public string currentFile = "";
        private Dictionary<string, string> pdfInfo;
    
        public Form1()
        {
            InitializeComponent();
        }

        private void loadDefaultPDF()
        {
            loadPdf(defaultPdf);
        }

        private void loadPdf(string pdfPath)
        {
            loadPdf(pdfPath, true);
        }

        private void loadPdf(string pdfPath, bool changeOriginal)
        {
            
            PdfiumViewer.PdfDocument pdfiumDoc = PdfiumViewer.PdfDocument.Load(pdfPath);
            pdfRenderer1.Load(pdfiumDoc);
            pdfRenderer1.Show();
            
            currentFile = pdfPath;
            if (changeOriginal)
            {
                originalFile = pdfPath;
            }
            
            this.Text = "BetaSign - " + originalFile;

            reader = new PdfReader(pdfPath);
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            if(Screen.AllScreens.Length > 1)
            {
                this.Left = Screen.AllScreens[1].Bounds.Left;
                this.Top = Screen.AllScreens[1].Bounds.Top;
            }

            this.WindowState = FormWindowState.Maximized;

            loadDefaultPDF();
        }

        private void showFileInfo()
        {
            FileInfo fileInfo = new FileInfo();
            fileInfo.reader = reader;
            fileInfo.filePath = currentFile;
            fileInfo.saveFileInfoFn = saveFileInfo;
            fileInfo.ShowDialog(this);
        }

        public void saveFileInfo(Dictionary<string, string> info)
        {
            pdfInfo = info;
        }

        private void fileInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showFileInfo();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void OpenFile()
        {
            openFileDialog1.Filter = "PDF Documents | *.pdf";
            openFileDialog1.Multiselect = false;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    loadPdf(openFileDialog1.FileName);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("File not found");
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        public void save(string path)
        {

            FileStream fsOut = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            FileStream fsIn = new FileStream(currentFile, FileMode.Open, FileAccess.Read);
            fsIn.CopyTo(fsOut);

            fsIn.Close();
            fsOut.Close();

        
        }

        public void saveAs()
        {
            saveFileDialog1.Filter = "PDF Documents | *.pdf";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                save(saveFileDialog1.FileName);
                loadPdf(saveFileDialog1.FileName, true);
            }
        }

        public byte[] captureSignature()
        {
            SigCtl sigCtl = new SigCtl();
            DynamicCapture dc = new DynamicCapture();
            DynamicCaptureResult res = dc.Capture(sigCtl, Properties.Settings.Default.DefaultName, Properties.Settings.Default.DefaultReason, null, null);

            if (res == DynamicCaptureResult.DynCaptOK)
            {
                SigObj sigObj = (SigObj)sigCtl.Signature;
                //sigObj.set_ExtraData("AdditionalData", "C# test: Additional data");

                String filename = System.IO.Path.GetTempFileName();
                try
                {
                    byte[] signature = sigObj.RenderBitmap(null, 400, 200, "image/png", 0.5f, 0xff0000, 0xffffff, 10.0f, 10.0f, RBFlags.RenderOutputBinary | RBFlags.RenderColor32BPP | RBFlags.RenderEncodeData | RBFlags.RenderBackgroundTransparent);
                    return signature;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }

            return null;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAs();   
        }

        private void signToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[] signature = captureSignature();
            if (signature == null) return;
            string newFile = signWithGraphic(signature);
            if (newFile == null) return;
            loadPdf(newFile, false);
        }
        
        private string signWithGraphic(byte[] signature)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = " PKCS#12 Files|*.pfx;*.p12";
            DialogResult dResult = ofd.ShowDialog();

            if(dResult != DialogResult.OK)
            {
                return null;
            }

            string pfxFile = ofd.FileName;

            string password = InputBox.show("Enter Password", true, this);

            if (password == null) return null;
            
            FileStream ksfs = new FileStream(pfxFile, FileMode.Open);

            Pkcs12Store pk12;

            try
            {
                pk12 = new Pkcs12Store(ksfs, password.ToCharArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Incorrect Passphrase - " + ex.Message);
                ksfs.Dispose();
                return null;
            }

            string alias = "";
            foreach (string al in pk12.Aliases) {
                if (pk12.IsKeyEntry(al) && pk12.GetKey(al).Key.IsPrivate)
                {
                    alias = al;
                    break;
                }
            }   

            Org.BouncyCastle.Pkcs.X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
            ICollection<X509Certificate> chain = new List<X509Certificate>();
            foreach (X509CertificateEntry c in ce)
            {
                chain.Add(c.Certificate);
            }

            AsymmetricKeyEntry pk = pk12.GetKey(alias);
            RsaPrivateCrtKeyParameters parameters = pk.Key as RsaPrivateCrtKeyParameters;

            string tmpFile = System.IO.Path.GetTempFileName();
           
            FileStream fs = new FileStream(tmpFile, FileMode.Create);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, fs, '\0');

            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.Reason = Properties.Settings.Default.DefaultReason;
            appearance.Location = Properties.Settings.Default.DefaultLocation;
            appearance.Contact = Properties.Settings.Default.DefaultContact;

            //uncomment this portion only
            //appearance.SignatureGraphic = iTextSharp.text.Image.GetInstance(signature);
            //appearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION;
            appearance.SetVisibleSignature(new iTextSharp.text.Rectangle(40, 110, 240, 210), 1, "Signature");
            //appearance.Certificate = chain[0]; to remain commented out

            /*
            PdfTemplate n2 = appearance.GetLayer(2);
            ColumnText ct = new ColumnText(n2);
            ct.SetSimpleColumn(n2.BoundingBox);
            string backgroundText = "Digitally signed by " + Properties.Settings.Default.DefaultName + "\nOn: " + appearance.SignDate.ToString() + "\nReason: " + appearance.Reason;
            iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph(backgroundText);
            ct.AddElement(paragraph);
            ct.Go();
            */
            string backgroundText = "Digitally signed by " + Properties.Settings.Default.DefaultName + "\nOn: " + appearance.SignDate.ToString() + "\nReason: " + appearance.Reason;
            appearance.Layer2Text = backgroundText;
            appearance.Image = iTextSharp.text.Image.GetInstance(signature);
            //appearance.ImageScale = 1;


            IExternalSignature pks = new PrivateKeySignature((ICipherParameters)parameters, DigestAlgorithms.SHA256);
            MakeSignature.SignDetached(appearance,pks, chain, null, null, null, 0, CryptoStandard.CADES);

            ksfs.Dispose();
            
            //stamper.Close();
            //fs.Close();

            return tmpFile;
        }

        private void showSignatureInfo()
        {
            SignatureInfo info = new SignatureInfo();
            info.loadSignInfo(currentFile);
            info.ShowDialog();
        }

        private void signatureInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showSignatureInfo();
        }


    }
}
