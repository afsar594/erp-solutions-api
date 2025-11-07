using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Inspire.Erp.Domain.Entities
{
    public class PaymentRequest
    {
        public int PaymentRequestSno { get; set; }
        public string PaymentRequestVoucherNo { get; set; }
        public DateTime? PaymentRequestDate { get; set; }
        public string PaymentRequestVoucherRef { get; set; }
        public double? PaymentRequestDbAmount { get; set; }
        public double? PaymentRequestFcDbAmount { get; set; }
        public string PaymentRequestNarration { get; set; }
        public int? PaymentRequestFsno { get; set; }
        public int? PaymentRequestUserId { get; set; }
        public int? PaymentRequestCurrencyId { get; set; }
        public int? PaymentRequestLocationId { get; set; }
        public bool? PaymentRequestDelStatus { get; set; }
        [JsonIgnore]
        [NotMapped]
        public virtual VouchersNumbers PaymentRequestVoucherNoNavigation { get; set; }

        [NotMapped]
        public List<PaymentRequestDetails> PaymentRequestDetails { get; set; }
        //[NotMapped]
        //public List<AccountsTransactions> AccountsTransactions { get; set; }
        //[NotMapped]
        //public List<AccountsTransactions> PaymentAllocationDetails { get; set; }
        //[NotMapped]
        //public List<Domain.Models.AllocationDetails> allocationDetails { get; set; }

    }
}
