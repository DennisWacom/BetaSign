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
using iTextSharp.text.pdf.security;
using Florentis;

namespace BetaSign
{
    public partial class SignatureInfo : Form
    {
        public SignatureInfo()
        {
            InitializeComponent();
        }

        public void loadSignInfo(string pdf_filename)
        {
            PdfReader reader = new PdfReader(pdf_filename);
            AcroFields fields = reader.AcroFields;
            int sigIndex = 1;
            SignatureImageExtractor extractor = new SignatureImageExtractor(reader);

            foreach(string sigFieldName in fields.GetSignatureNames()){

                PdfImageObject image = extractor.extractImage(sigFieldName);
                MemoryStream ms = new MemoryStream(image.GetImageAsBytes());

                SigObj sig = new SigObj();

                ReadEncodedBitmapResult result = sig.ReadEncodedBitmap(ms.ToArray());
                if (result == ReadEncodedBitmapResult.ReadEncodedBitmapOK)
                {
                    //MessageBox.Show(sig.Who + " " + sig.Why + " " + sig.When);
                    treeView1.BeginUpdate();
                    treeView1.Nodes.Add("Signature " + sigIndex);
                    treeView1.Nodes[sigIndex - 1].Nodes.Add("Name: " + sig.Who);
                    treeView1.Nodes[sigIndex - 1].Nodes.Add("Reason: " + sig.Why);
                    treeView1.Nodes[sigIndex - 1].Nodes.Add("Timestamp: " + sig.When);

                    treeView1.Nodes[sigIndex - 1].Nodes.Add("Digitizer: " + sig.get_AdditionalData(CaptData.CaptDigitizer));
                    treeView1.Nodes[sigIndex - 1].Nodes.Add("Digitizer Driver: " + sig.get_AdditionalData(CaptData.CaptDigitizerDriver));
                    treeView1.Nodes[sigIndex - 1].Nodes.Add("Machine OS: " + sig.get_AdditionalData(CaptData.CaptMachineOS));
                    treeView1.Nodes[sigIndex - 1].Nodes.Add("Network Card: " + sig.get_AdditionalData(CaptData.CaptNetworkCard));

                    treeView1.Nodes[sigIndex - 1].Nodes.Add("Signature Covers whole document: " + fields.SignatureCoversWholeDocument(sigFieldName).ToString());
                    treeView1.Nodes[sigIndex - 1].Nodes.Add("Document Revision: " + fields.GetRevision(sigFieldName).ToString() + " of " + fields.TotalRevisions.ToString());

                    PdfPKCS7 pkcs7 = fields.VerifySignature(sigFieldName);

                    treeView1.Nodes[sigIndex - 1].Nodes.Add("Integrity Check OK? " + pkcs7.Verify().ToString());

                    treeView1.EndUpdate();

                    sigIndex = sigIndex + 1;
                }
                ms.Close();
            }

            treeView1.ExpandAll();
 
        }

    }
}
