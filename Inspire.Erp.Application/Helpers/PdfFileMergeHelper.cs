using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace Inspire.Erp.Application.Helpers
{
    public static class PdfFileMergeHelper
    {
        
        
        public static byte[] MergePdfsFromPaths(List<string> filePaths, string password = null)
        {
            if (filePaths == null || filePaths.Count == 0)
                throw new ArgumentException("No PDF files provided.", nameof(filePaths));

            using var outputStream = new MemoryStream();
            using var writer = new PdfWriter(outputStream);
            using var outputDoc = new iText.Kernel.Pdf.PdfDocument(writer);
            var merger = new PdfMerger(outputDoc);
            var fileDirectory = Directory.GetCurrentDirectory();
            foreach (var path in filePaths)
            {
                try
                {
                    var fullPath = fileDirectory + path.Replace("/", "\\");
                    if (!File.Exists(fullPath))
                    {
                        Console.WriteLine($"⚠️ File not found: {path}");
                        continue;
                    }

                    var readerProps = new ReaderProperties();
                    if (!string.IsNullOrEmpty(password))
                        readerProps.SetPassword(System.Text.Encoding.UTF8.GetBytes(password));

                    using var reader = new iText.Kernel.Pdf.PdfReader(fullPath, readerProps);
                    using var srcDoc = new iText.Kernel.Pdf.PdfDocument(reader);
                    merger.Merge(srcDoc, 1, srcDoc.GetNumberOfPages());
                }
                catch (PdfException ex)
                {
                    Console.WriteLine($"⚠️ Skipped invalid/corrupted PDF '{path}' → {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Unexpected error merging '{path}' → {ex.Message}");
                }
            }

            outputDoc.Close();
            return outputStream.ToArray();
        }
    }
}
