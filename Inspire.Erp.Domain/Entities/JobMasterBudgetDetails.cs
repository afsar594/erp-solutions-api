using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Inspire.Erp.Domain.Entities
{
    public partial class JobMasterBudgetDetails
    {
        public int JobMasterBudgetDetailsDtlId { get; set; }
        public int? JobMasterBudgetDetailsJobId { get; set; }
        public string JobMasterNo { get; set; }
        public int? JobMasterBudgetDetailsSno { get; set; }
        public int? JobMasterBudgetDetailsBudId { get; set; }
        public decimal? JobMasterBudgetDetailsBudAmount { get; set; }
        public bool? JobMasterBudgetDetailsDelStatus { get; set; }
        [JsonIgnore]
        public virtual JobMaster JobMasterBudgetDetailsJob { get; set; }
    }
}
