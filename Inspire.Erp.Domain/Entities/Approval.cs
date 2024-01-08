using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities
{
    public class Approval
    {
        public int Id { get; set; }
        public string? VoucherType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public int? LocationId { get; set; }
        public List<ApprovalDetail> ApprovalDetails { get; set; }
    }
}
