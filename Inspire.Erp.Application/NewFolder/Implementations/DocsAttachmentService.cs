using Inspire.Erp.Application.NewFolder.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.NewFolder.Implementations
{
    public class DocsAttachmentService : IDocsAttachmentService
    {
        private IRepository<DocsAttachment> docrepository;
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
        public IEnumerable<DocsAttachment> GetAllDocumentById(int id ,string type )
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


    }
}
