using Inspire.Erp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Common
{
    public class VoucherApprivalDto
    {
        public VoucherApprivalDto()
        {
            VoucherApprovers = new List<VoucherApprover>();
        }
        public int VoucherId { get; set; }
        public EnumOrderForms VoucherType { get; set; }
        public string VoucherNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public string LocationName { get; set; }
        public int JobId { get; set; }
        public string JobName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }

        public List<VoucherApprover> VoucherApprovers { get; set; }
    }
    public class VoucherApprover
    {
        public int ApproverId { get; set; }
        public EnumApproverLevels  ApproverLevel { get; set; }
        public EnumApprovalStatus Status { get; set; }
    }
}
