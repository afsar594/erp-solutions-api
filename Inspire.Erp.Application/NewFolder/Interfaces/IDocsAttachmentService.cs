using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Domain.Modals;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.NewFolder.Interfaces
{
    public interface IDocsAttachmentService
    {
        //public IEnumerable<Docs_attachment> GetDocumentsById(int id,int docsId);
        public IEnumerable<DocsAttachment> GetDocumentstById(int DocumentId, int DocumentTypeId ,string type);

        //public Task<FileResultDto> GetPdfById(int id);
        public Task<FileResultDto> GetPdfById(int id);
        public void SaveDocument(DocsAttachment Documnet);

        public IEnumerable<DocsAttachment> GetAllDocumentById(int id,string type);
        public IEnumerable<DocsAttachment> DeleteDocument(int id);
        public Task<Response<List<JobDocumentsDto>>> GetJobDocuments(int id, string type);
        public Task<FileResultDto> DownloadAllFilesAsOnePdf(int id, string type);
        public Task<object> CopyAttachmentsFromPRToPO(int prId, int poId, List<int> prFileIds);

    }
}
