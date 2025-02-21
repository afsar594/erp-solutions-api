using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities
{
    public partial class JobDocuments
    {
        public int JobDocumentId { get; set; }
        public int JobDocumentJobId { get; set; }
        public string? JobDocumentName { get; set; }
        public bool? JobDocumentDelStatus { get; set; }
    }
    public class FileResultDto
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
    }
}
