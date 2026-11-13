using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Inspire.Erp.Domain.Entities
{
    public class PaymentRequestDetails
    {
        public int PaymentRequestDetailsId { get; set; }   
        public string PaymentRequestDetailsVoucherNo { get; set; } 
        public int? PaymentRequestDetailsSno { get; set; }
        public string PaymentRequestDetailsAcNo { get; set; }
        public double? PaymentRequestDetailsDbAmount { get; set; } 
        public double? PaymentRequestDetailsDbFcAmount { get; set; }
        public int? PaymentRequestDetailsDepartmentId { get; set; }
        public string PaymentRequestDetailsNarration { get; set; } 
        public int? PaymentRequestDetailsFsno { get; set; } 
        public int? PaymentRequestDetailsLocationId { get; set; } 
        public bool? PaymentRequestDetailsDelStatus { get; set; }
    }

}
