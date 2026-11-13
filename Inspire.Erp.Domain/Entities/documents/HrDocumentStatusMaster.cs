using IInspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities.documents
{
    public partial class HrDocumentStatusMaster : BaseEntity
    {
        public long DocStatusId { get; set; }
        public string DocStatus { get; set; }
    }
}
