using Inspire.Erp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Common
{
    public class ApprovalFilterModel
    {
        public ApprovalFilterModel()
        {
            Type = new List<EnumOrderForms>();
        }
        public EnumApprovalStatus Status { get; set; } = EnumApprovalStatus.None;
        public List<EnumOrderForms> Type { get; set; }
        public int UserId { get; set; }
        public int JobId { get; set; }
        public bool IsDate { get; set; }
        public bool IsUserRequired { get; set; } = true;
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
    }
}
