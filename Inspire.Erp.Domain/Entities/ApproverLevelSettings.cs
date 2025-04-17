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

        [Required]
        [MaxLength(50)]
        public EnumOrderForms FormType { get; set; }

        [Required]
        [MaxLength(50)]
        public EnumApproverLevels ApproverLevel { get; set; }

        [Required]
        public long ApproverId { get; set; }

        [Required]
        public bool OptEmail { get; set; } = false;

        [Required]
        public int NotificationDays { get; set; }

        [Required]
        [Column(TypeName = "numeric(18, 2)")]
        public decimal MinAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "numeric(18, 2)")]
        public decimal MaxAmount { get; set; } = 0;

        [ForeignKey("ApproverId")]
        public virtual UserFile Approver { get; set; }
    }
}

