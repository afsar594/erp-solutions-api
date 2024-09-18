using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities.documents
{
    public partial class HrDocumentTypeViewModel
    {
        public long DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public bool DefaultType { get; set; }
        public bool? AppDocument { get; set; }
    }
}
