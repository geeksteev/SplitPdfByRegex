using System;
using System.IO;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace SplitPdfByRegex
{
    class Program
    {
        static void Main(string[] args)
        {
            // Change variables based upon folder/file locations
            string workingFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CommanderPayReport";
            string srcPdfPath = Path.Combine(workingFolder + @"\PayReport\");
            string srcSplitPdfPath = Path.Combine(workingFolder + @"\files\");
            string dstFinalPdfPath = Path.Combine(workingFolder + @"\results\");
            string dstBackupPath = Path.Combine(workingFolder + @"\backup\");

            SplitPdfIntoIndividualPages(workingFolder, srcPdfPath, srcSplitPdfPath);
            SearchAndMerge(srcSplitPdfPath, dstFinalPdfPath);
            CleanupFiles(workingFolder, srcPdfPath, dstBackupPath);
        }
        static void SplitPdfIntoIndividualPages(string workingFolder, string srcPdfPath, string dstSplitPdfPath)
        {
            foreach (var file in Directory.GetFiles(srcPdfPath))
            {
                string backupDir = workingFolder + @"backup";

                PdfCopy copy;
                //create PdfReader object
                PdfReader reader = new PdfReader(file);

                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    //create Document object
                    Document document = new Document();
                    copy = new PdfCopy(document, new FileStream(dstSplitPdfPath + page + ".pdf", FileMode.Create));
                    //open the document 
                    document.Open();
                    //add page to PdfCopy 
                    copy.AddPage(copy.GetImportedPage(reader, page));
                    //close the document object
                    document.Close();
                }
            }
        }
        static void SearchAndMerge(string srcSplitPdfPath, string dstFinalPdfPath)
        {
            // Regex search strings
            var searchText = @"((NH)[A-Z0-9]{5})";
            var reg = new Regex(searchText);

            foreach (var file in Directory.EnumerateFiles(srcSplitPdfPath, "*.pdf"))
            {
                using (var pdfReader = new PdfReader(file))
                {
                    for (var page = 1; page <= pdfReader.NumberOfPages; page++)
                    {
                        //Get the text from the page
                        var currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, new SimpleTextExtractionStrategy());

                        //currentText.IndexOf("", StringComparison.InvariantCultureIgnoreCase);
                        currentText.Contains("");

                        //Match our pattern against the extracted text
                        //var matches = reg.Matches(currentText);
                        var match = reg.Match(currentText);

                        //This is the file path that we want to target
                        var destFile = Path.Combine(dstFinalPdfPath, match.ToString() + ".pdf");

                        //If the file doesn't already exist then just copy the file and move on
                        if (!File.Exists(destFile))
                        {
                            System.IO.File.Copy(file, destFile);
                            continue;
                        }

                        using (var ms = new MemoryStream())
                        {
                            //Use a wrapper helper provided by iText
                            var cc = new PdfConcatenate(ms);

                            //Open for writing
                            cc.Open();

                            //Import the existing file
                            using (var subReader = new PdfReader(destFile))
                            {
                                cc.AddPages(subReader);
                            }

                            using (var subReader = new PdfReader(file))
                            {
                                cc.AddPages(subReader);
                            }

                            //Close for writing
                            cc.Close();

                            //Erase our exisiting file
                            File.Delete(destFile);

                            //Write our new file
                            File.WriteAllBytes(destFile, ms.ToArray());
                        }
                    }
                }
            }
        }
        static void CleanupFiles(string workingFolder, string srcPdfPath, string dstBackupPath)
        {
            foreach (var file in Directory.GetFiles(srcPdfPath))
            {
                string fileName = Path.GetFileName(file);
                string NewPath = dstBackupPath + fileName;
                File.Move(file, NewPath);
            }
        }
    }
}
