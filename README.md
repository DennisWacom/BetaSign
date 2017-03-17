# BetaSign
BetaSign is a demo application to allow users to sign pdf documents with a digital certificate (.pfx file) and a signature image, captured on a Wacom signature pad, onto the pdf.


## Dependencies
1. [iTextSharp](https://www.nuget.org/packages/iTextSharp/)
2. [PdfiumViewer 2.6.1](https://www.nuget.org/packages/PdfiumViewer/)
3. [Wacom Signature SDK](http://signature.wacom.eu/products/software/software-development-kits-sdks/)
4. Wacom hardware (Any [STU Signpads](http://www.wacom.com/en-sg/enterprise/business-solutions/hardware/signature-pads), [Pen Display](http://www.wacom.com/en-sg/enterprise/business-solutions/hardware/pen-displays), [Pen Tablets](http://www.wacom.com/en-sg/products/pen-tablets))

## Features
1. Open pdf
2. Save pdf
3. Sign pdf with Wacom Signature SDK
4. Display signature information

## Limitations
1. Location of signature on the pdf is hardcoded. No way for user to specify where the signature is placed in the pdf.
