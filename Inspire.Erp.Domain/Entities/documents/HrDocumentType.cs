using IInspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities.documents
{
    public partial class HrDocumentType : BaseEntity
    {
        public long DocumentTypeId { get; set; }
        public string? DocumentType { get; set; }
        public bool DefaultType { get; set; }
        public bool? AppDocument { get; set; }
      public bool Deletestatus { get; set; }

      public string DocumentvoucherNo { get; set; }
        
    }
    
    


    }
    public partial class HrDocumentTypeFilter : BaseEntity
    {
        public string? MasterType { get; set; }
        public DateTime? todate { get; set; }
        public DateTime? fromdate { get; set; }
        public int? companyid { get; set; }
        public string? DocumentType { get; set; }
        public int? statusId { get; set; }
    

}
