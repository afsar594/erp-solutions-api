using IInspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities.documents
{
    public partial class HrDocumentImages : BaseEntity
    {
        public long DocImageId { get; set; }
        public long? DocumentId { get; set; }
        public string DocImage { get; set; }
        public string Extension { get; set; }
        public string DocName { get; set; }

        //public virtual HrDocumentMaster Document { get; set; }
    }

    public class DTOHrDocumentImages
    {
        public long DocImageId { get; set; }
        public long? DocumentId { get; set; }
        public string DocImage { get; set; }
        public string Extension { get; set; }
        public string DocName { get; set; }
    }
}