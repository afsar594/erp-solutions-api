using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
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
    }
}
