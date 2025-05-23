using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.Collections.Generic;
using System.IO;

namespace Inspire.Erp.Application.Helpers
{
    public static class PdfFileMergeHelper
    {
        public static byte[] MergePdfsFromPaths(List<string> paths)
        {
            using var outputDocument = new PdfDocument();

            var fileDirectory = Directory.GetCurrentDirectory();
            foreach (var fPath in paths)
            {
                var _path = fileDirectory + fPath.Replace("/","\\");
                if (!File.Exists(_path)) continue;
                using var stream = File.OpenRead(_path);
                using var inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
                
                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    outputDocument.AddPage(inputDocument.Pages[i]);
                }
            }

            using var memoryStream = new MemoryStream();
            outputDocument.Save(memoryStream, false);
            return memoryStream.ToArray();
        }
    }
}
