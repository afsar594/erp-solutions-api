using System;
using System.Collections.Generic;

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

        public virtual JobMaster JobMasterJobWiseBankGuaranteesJob { get; set; }
    }
}
