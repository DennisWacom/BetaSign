This application uses PDFium, PDFiumViewer, and iTextSharp to demonstrate digital signing of PDF documents with private key (.pkx or .p12) file.

Please change the values in the solution/project properties before signing.
Upon clicking on the sign button, the program will prompt user to sign on the signature tablet, then use the private key specified in the properties file to sign the document. The program will prompt the user for the password of the private key to continue. The signed file will be saved in a temporary location and will not overwrite the current document until the user saves and overwrite the file.

This software includes open source software itext, a commercial license is to be purchased from itext if you want to use it for commercial purposes.