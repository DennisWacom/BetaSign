using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace BetaSign
{
    public class SignatureImageExtractor
    {
        PdfReader reader;

        public SignatureImageExtractor(PdfReader r)
        {
            reader = r;
        }

        public PdfImageObject extractImage(string fieldname)
        {
            MyImageRenderer listener = new MyImageRenderer();
            PdfDictionary sigFieldDict = reader.AcroFields.GetFieldItem(fieldname).GetMerged(0);
            PdfDictionary appearanceDict = sigFieldDict.GetAsDict(PdfName.AP);
            PdfStream normalAppearance = appearanceDict.GetAsStream(PdfName.N);

            PdfDictionary resourceDict = normalAppearance.GetAsDict(PdfName.RESOURCES);
            PdfContentStreamProcessor processor = new PdfContentStreamProcessor(listener);
            processor.ProcessContent(ContentByteUtils.GetContentBytesFromContentObject(normalAppearance), resourceDict);

            return listener.image;
        }

    }

    class MyImageRenderer : IRenderListener{
        public PdfImageObject image = null;

        public void BeginTextBlock() { }
        public void EndTextBlock() { }
        public void RenderText(TextRenderInfo renderInfo) { }
        public void RenderImage(ImageRenderInfo renderInfo)
        {
            try
            {
                image = renderInfo.GetImage();
            }
            catch (System.IO.IOException)
            {

            }
        }
    }
}
