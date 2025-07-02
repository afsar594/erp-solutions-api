using Inspire.Erp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Common
{
    public class POPRApprovalDto
    {
        public int OrderId { get; set; }
        public EnumOrderForms OrderType { get; set; }
        public EnumApprovalStatus ApprovalStatus { get; set; }
        public EnumApproverLevels ApproverLevel { get; set; }
        public string Remarks { get; set; }
        public int JobId { get; set; }
        public int? UserId { get; set; }
    }
}
