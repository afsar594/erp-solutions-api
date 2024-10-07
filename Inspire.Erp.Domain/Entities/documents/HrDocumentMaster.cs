using IInspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities.documents
{
    public partial class DocumentMaster : BaseEntity
    {
        public long DocumentId { get; set; }
        public long DocumentTypeId { get; set; }
        public long? StaffId { get; set; }
        public long? CompanyId { get; set; }
        public long? VehicleId { get; set; }
        public long? OtherId { get; set; }
        public string? MasterType { get; set; }
        public string? DocName { get; set; }
        public string DocNo { get; set; }
        public string? Remarks { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpDate { get; set; }
        public string? DocImage { get; set; }
        public bool? Status { get; set; }
        public long? DocStatusId { get; set; }
        public string DocumentvoucherNo { get; set; }
    }

    public partial class Documentfilter : BaseEntity
    {
        public long? DocumentTypeId { get; set; }
        public long? CompanyId { get; set; }
        public DateTime? IssueDateto { get; set; }
        public DateTime? IssueDatefrom { get; set; }
        public bool isDateChecked { get; set; }
    }
    public partial class DocumentMasterlist : BaseEntity
    {
        public long DocumentTypeId { get; set; }
        public long? CompanyId { get; set; }
        public string? Companyname { get; set; }
        public string? Documenttype { get; set; }
        public long? VehicleId { get; set; }
        public string MasterType { get; set; }
        public string DocName { get; set; }
        public string DocNo { get; set; }
        public string Remarks { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpDate { get; set; }        
        public string? DocImage { get; set; }
        public bool? Status { get; set; }
        public string DocumentvoucherNo { get; set; }
    }

}
