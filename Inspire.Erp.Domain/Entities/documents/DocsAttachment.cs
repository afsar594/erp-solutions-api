using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities.documents
{
    public class DocsAttachment
    {
        public int DocumentId { get; set; }
        public int DocumentVocucherId { get; set; }
        public string? DocumentName { get; set; }

        public string? DocumentType { get; set; }
        public bool? DocumentDelStatus { get; set; }
    }
}
