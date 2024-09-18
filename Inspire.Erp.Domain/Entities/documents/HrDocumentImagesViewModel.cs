using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities.documents
{
    public partial class HrDocumentImagesViewModel
    {
        public long DocImageId { get; set; }
        public long? DocumentId { get; set; }
        public string DocImage { get; set; }
        public string Extension { get; set; }
        public string DocName { get; internal set; }

        //public virtual HrDocumentMaster Document { get; set; }
    }
}
