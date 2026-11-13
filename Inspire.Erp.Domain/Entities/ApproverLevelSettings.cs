using Inspire.Erp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Inspire.Erp.Domain.Entities
{

    public class ApproverLevelSettings
    {
        [Key]
        public int Id { get; set; }
        public EnumOrderForms FormType { get; set; }
        public EnumApproverLevels ApproverLevel { get; set; }
        public long ApproverId { get; set; }
        public bool OptEmail { get; set; }
        public int NotificationDays { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public int? JobId { get; set; }
        public string MetaData { get; set; }
        public int SNo { get; set; }

        [ForeignKey("ApproverId")]
        public virtual UserFile Approver { get; set; }
    }
}

