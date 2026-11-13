using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities.documents
{
    public partial class HrDocumentMasterViewModel
    {
        //public HrDocumentMaster()
        //{
        //    HrDocumentImages = new HashSet<HrDocumentImages>();
        //}

        public long DocumentId { get; set; }
        public long DocumentTypeId { get; set; }
        public long? StaffId { get; set; }
        public long? CompanyId { get; set; }
        public long? VehicleId { get; set; }
        public long? OtherId { get; set; }
        public string MasterType { get; set; }
        public string DocName { get; set; }
        public string DocNo { get; set; }
        public string Remarks { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpDate { get; set; }
        public byte[] DocImage { get; set; }
        public bool? Status { get; set; }
        public long? DocStatusId { get; set; }

        //public virtual ICollection<HrDocumentImages> HrDocumentImages { get; set; }
    }

}
