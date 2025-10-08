using Inspire.Erp.Application.NewFolder.Implementations;
using Inspire.Erp.Application.NewFolder.Interfaces;
using Inspire.Erp.Domain.Entities.documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Helpers
{
    public  class DownloadAndSaveVoucherReport
    {
        IDocsAttachmentService docsAttachmentService;

        public DownloadAndSaveVoucherReport(IDocsAttachmentService docsAttachmentService)
        {
            this.docsAttachmentService = docsAttachmentService;
        }

        public  async Task<string> DownloadedFileAsync(string reportUrl, string fileType, string fileNo,int fileId,string vNO, bool isUpdate = false)
        {
            var sql = fileType == "PurchaseOrder"
                                          ? $"select  * from VR_PurchaseOrder where purchaseorderid = {fileId}"
                                          : $"select  * from vw_PurchaseRequisition where purchaserequisitionid = {fileId}";

            Console.WriteLine($"Executing report SQL: {sql}");


            var reportFilter = new
            {
                ReportName = fileType,
                SelectionFormula = "",
                Parameters = new List<string>(),
                Queries = new List<string>() { sql },
                SP_Queries = new List<string>(),
                ExportType = 5
            };

            var json = JsonSerializer.Serialize(reportFilter);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PutAsync(reportUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                        var pdfDir = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "Pdfs");
                        if (!Directory.Exists(pdfDir))
                            Directory.CreateDirectory(pdfDir);

                        var filePath = Path.Combine(pdfDir, $"{fileType}_{vNO}.pdf");
                        
                        if(File.Exists(filePath))
                            File.Delete(filePath); // Delete existing file if it exists

                        await File.WriteAllBytesAsync(filePath, pdfBytes);
                        Console.WriteLine($"Report saved to: {filePath}");
                       
                        if (isUpdate)
                            return filePath;

                        string fileName = Path.GetFileName(filePath);
                        DocsAttachment Documents = new DocsAttachment();
                        Documents.DocumentName = "/Upload/Pdfs/" + fileName;
                        Documents.DocumentDelStatus = false;
                        Documents.DocumentVocucherId = fileId;
                        Documents.DocumentType = fileType == "PurchaseOrder" ? "PO" : "PR";
                        docsAttachmentService.SaveDocument(Documents);

                        return filePath;
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
