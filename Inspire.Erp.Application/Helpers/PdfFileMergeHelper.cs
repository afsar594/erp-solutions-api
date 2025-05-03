using Microsoft.AspNetCore.Http;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

namespace Inspire.Erp.Application.Helpers
{
    public static class PdfFileMergeHelper
    {
        //public static byte[] MergePdfsFromPaths(List<string> paths)
        //{
        //    using var outputDocument = new PdfDocument();

        //    var fileDirectory = Directory.GetCurrentDirectory();
        //    foreach (var fPath in paths)
        //    {
        //        fPath = Path.Combine(fileDirectory, fPath);
        //        using var stream = File.OpenRead(fPath);
        //        using var inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
        //        for (int i = 0; i < inputDocument.PageCount; i++)
        //        {
        //            outputDocument.AddPage(inputDocument.Pages[i]);
        //        }
        //    }

        //    using var memoryStream = new MemoryStream();
        //    outputDocument.Save(memoryStream, false);
        //    return memoryStream.ToArray();
        //}
    }
}
