using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Inspire.Erp.Domain.Entities
{
    public partial class JobMasterJobWiseBankGuarantees
    {
        public int JobMasterJobWiseBankGuaranteesDtlId { get; set; }
        public int JobMasterJobWiseBankGuaranteesJobId { get; set; }
        public string JobMasterNo { get; set; }
        public long JobMasterJobWiseBankGuaranteesBgid { get; set; }
        public decimal? JobMasterJobWiseBankGuaranteesAmount { get; set; }
        public int? JobMasterJobWiseBankGuaranteesSlNo { get; set; }
        public bool? JobMasterJobWiseBankGuaranteesDelStatus { get; set; }
        public DateTime? Job_Master_JobWiseBankGuarantees_Valid_From_Date { get; set; }
        public string Job_Master_JobWiseBankGuarantees_Remarks { get; set; }

        [JsonIgnore]
        public virtual JobMaster JobMasterJobWiseBankGuaranteesJob { get; set; }
    }
}
