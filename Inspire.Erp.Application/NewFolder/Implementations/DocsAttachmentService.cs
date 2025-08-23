using Inspire.Erp.Application.Helpers;
using Inspire.Erp.Application.NewFolder.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.NewFolder.Implementations
{
    public class DocsAttachmentService : IDocsAttachmentService
    {
        private readonly IRepository<DocsAttachment> docrepository;
        private readonly string _pdfDirectory = @"Upload//Pdfs";
        public DocsAttachmentService(IRepository<DocsAttachment> _docsrepository)
        {
            docrepository = _docsrepository;
        }

        public void SaveDocument(DocsAttachment Documnet)
        {
            docrepository.Insert(Documnet);
        }
        public IEnumerable<DocsAttachment> GetDocumentstById(int DocumentId, int DocumenttypeId, string Type)
        {
            var doc = docrepository.GetAsQueryable().AsNoTracking().Where(a => a.DocumentId == Convert.ToInt32(DocumentId) && a.DocumentVocucherId == DocumenttypeId && a.DocumentType == Type).ToList();
            return doc;
        }
        public IEnumerable<DocsAttachment> GetAllDocumentById(int id, string type)
        {
            try
            {
                var doc = docrepository.GetAsQueryable().Where(k => k.DocumentVocucherId == id && k.DocumentDelStatus == false && k.DocumentType == type).ToList();
                return doc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<DocsAttachment> DeleteDocument(int Id)
        {
            try
            {
                var dataObj = docrepository.GetAsQueryable().Where(o => o.DocumentId == Id).FirstOrDefault();
                dataObj.DocumentDelStatus = true;
                docrepository.Update(dataObj);
                return docrepository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<FileResultDto> GetPdfById(int id)
        {
            if (!Directory.Exists(_pdfDirectory))
            {
                return null;
            }

            string[] allowedExtensions = { ".pdf", ".docx", ".xlsx" };
            var filePath = Directory.EnumerateFiles(_pdfDirectory)
                .FirstOrDefault(f => allowedExtensions.Contains(Path.GetExtension(f).ToLower()) &&
                                     Path.GetFileName(f).Contains(id.ToString()));

            if (filePath == null)
            {
                return null;
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath);

            return new FileResultDto
            {
                FileName = Path.GetFileName(filePath),
                FileBytes = fileBytes,
                ContentType = GetMimeType(filePath)
            };
        }

        // Helper method to get the correct MIME type
        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream" // Default binary file type
            };
        }
        public async Task<Response<List<JobDocumentsDto>>> GetJobDocuments(int id, string type)
        {
            var query = docrepository
                .GetAsQueryable()
                .AsNoTracking()
                .Where(x => x.DocumentVocucherId == id &&
                            x.DocumentType.ToLower() == type.ToLower() &&
                            (x.DocumentDelStatus == null || x.DocumentDelStatus == false));

            var docs = await query.Select(x => new JobDocumentsDto
            {
                FileId = x.DocumentId,
                FileName = Path.GetFileName(x.DocumentName),
                FilePath = Path.GetFileName(x.DocumentName)
            }).ToListAsync();

            if (docs == null || docs.Count == 0)
                return Response<List<JobDocumentsDto>>.Fail(null, "No documents found.");

            return Response<List<JobDocumentsDto>>.Success(docs, "Documents found.");
        }

        public async Task<FileResultDto> DownloadAllFilesAsOnePdf(int id, string type)
        {
            try
            {
                var jobDocs = await docrepository
                                     .GetAsQueryable()
                                     .AsNoTracking()
                                     .Where(x => x.DocumentVocucherId == id
                                                  && x.DocumentType.ToLower() == type.ToLower()
                                                  && (x.DocumentDelStatus == null || x.DocumentDelStatus == false))
                                     .OrderBy(x => x.DocumentId)
                                     .Select(x => x.DocumentName)
                                     .ToListAsync();
                if (!jobDocs.Any())
                    return new FileResultDto();

                var mergedPdf = PdfFileMergeHelper.MergePdfsFromPaths(jobDocs);

                return new FileResultDto
                {
                    FileName = $"{GetFileName(type)}.pdf",// "downloadedFile",
                    FileBytes = mergedPdf,
                    ContentType = "application/pdf"
                };
            }
            catch (Exception)
            {
                return new FileResultDto();
            }
        }

        private string GetFileName(string type)
        {
            return type switch
            {
                "PO" => "PurhchaseOrderFiles",
                "PR" => "PurhchaseRequisitionFiles",
                "JobMaster" => "JobMasterFiles",
                _ => "UnknownTypeFiles"
            };
        }

        public async Task<object> CopyAttachmentsFromPRToPO(int prId, int poId, List<int> prFileIds)
        {
            try
            {
                var prAttachments = await docrepository
                                     .GetAsQueryable()
                                     .AsNoTracking()
                                     .Where(a => a.DocumentVocucherId == prId
                                                && a.DocumentType == "PR"
                                                && prFileIds.Contains(a.DocumentId))
                                     .ToListAsync();

                var poAttachments = prAttachments.Select(a => new DocsAttachment
                {
                    DocumentVocucherId = poId,
                    DocumentName = a.DocumentName,
                    DocumentType = "PO",
                    DocumentDelStatus = false
                }).ToList();
                await docrepository.InsertListAsync(poAttachments);

                return new { success = true, message = "Selected files transfered to Purhcase Order." };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Response<List<JobDocumentsDto>>> GetExcludedDocumentsAsync(int sourceVoucherId, string sourceType, int excludeVoucherId, string excludeType)
        {
            var excludedNames = await docrepository
                                      .GetAsQueryable()
                                      .AsNoTracking()
                                      .Where(x => x.DocumentVocucherId == excludeVoucherId && x.DocumentType == excludeType && (x.DocumentDelStatus == null || x.DocumentDelStatus == false))
                                      .Select(x => x.DocumentName)
                                      .ToListAsync();

            var result = await docrepository
                                    .GetAsQueryable()
                                    .AsNoTracking()
                                    .Where(x => x.DocumentVocucherId == sourceVoucherId &&
                                                x.DocumentType == sourceType &&
                                                !excludedNames.Contains(x.DocumentName) && (x.DocumentDelStatus == null || x.DocumentDelStatus == false))
                                    .Select(x => new JobDocumentsDto
                                    {
                                        FileId = x.DocumentId,
                                        FileName = Path.GetFileName(x.DocumentName),
                                        FilePath = Path.GetFileName(x.DocumentName)
                                    }).ToListAsync();
            if (result == null || result.Count == 0)
                return Response<List<JobDocumentsDto>>.Fail(null, "No documents found.");

            return Response<List<JobDocumentsDto>>.Success(result, "Documents found.");
        }



    }
}
