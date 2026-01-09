using Inspire.Erp.Application.NewFolder.Interfaces;
using Inspire.Erp.Domain.Entities.documents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Helpers
{
    public class DownloadAndSaveVoucherReport
    {
        private readonly IDocsAttachmentService _docsAttachmentService;
        private readonly ILogger<DownloadAndSaveVoucherReport> _logger;

        public DownloadAndSaveVoucherReport(
            IDocsAttachmentService docsAttachmentService,
            ILogger<DownloadAndSaveVoucherReport> logger)
        {
            _docsAttachmentService = docsAttachmentService;
            _logger = logger;
        }

        public async Task<string> DownloadedFileAsync(
            string reportUrl,
            string fileType,
            string fileNo,
            int fileId,
            string vNO,
            bool isUpdate = false)
        {
            _logger.LogInformation(
                "Starting voucher report download. FileType: {FileType}, FileId: {FileId}, VoucherNo: {VoucherNo}, IsUpdate: {IsUpdate}",
                fileType, fileId, vNO, isUpdate);

            string sql = fileType == "PurchaseOrder"
                ? $"select * from VR_PurchaseOrder where purchaseorderid = {fileId}"
                : $"select * from vw_PurchaseRequisition where purchaserequisitionid = {fileId}";

            _logger.LogDebug("Generated SQL for report: {Sql}", sql);

            var reportFilter = new
            {
                ReportName = fileType,
                SelectionFormula = "",
                Parameters = new List<string>(),
                Queries = new List<string> { sql },
                SP_Queries = new List<string>(),
                ExportType = 5
            };

            var json = JsonSerializer.Serialize(reportFilter);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogDebug("Serialized report payload successfully.");

            try
            {
                using var client = new HttpClient();

                _logger.LogInformation("Sending report request to URL: {ReportUrl}", reportUrl);

                var response = await client.PutAsync(reportUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning(
                        "Report API call failed. StatusCode: {StatusCode}, Response: {Response}",
                        response.StatusCode, error);

                    return string.Empty;
                }

                _logger.LogInformation("Report generated successfully. Downloading PDF.");

                var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                var pdfDir = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "Pdfs");

                if (!Directory.Exists(pdfDir))
                {
                    Directory.CreateDirectory(pdfDir);
                    _logger.LogInformation("Created directory: {Directory}", pdfDir);
                }

                var filePath = Path.Combine(pdfDir, $"{fileType}_{vNO}.pdf");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("Existing file deleted: {FilePath}", filePath);
                }

                await File.WriteAllBytesAsync(filePath, pdfBytes);
                _logger.LogInformation("PDF saved successfully at: {FilePath}", filePath);
             
                if (!isUpdate)
                {
                    var document = new DocsAttachment
                    {
                        DocumentName = "/Upload/Pdfs/" + Path.GetFileName(filePath),
                        DocumentDelStatus = false,
                        DocumentVocucherId = fileId,
                        DocumentType = fileType == "PurchaseOrder" ? "PO" : "PR"
                    };

                    _docsAttachmentService.SaveDocument(document);

                    _logger.LogInformation(
                        "Document metadata saved successfully. VoucherId: {VoucherId}, Type: {DocumentType}",
                        fileId, document.DocumentType);
                }


                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Exception occurred while downloading voucher report. FileType: {FileType}, FileId: {FileId}",
                    fileType, fileId);

                return string.Empty;
            }
        }
    }
}
