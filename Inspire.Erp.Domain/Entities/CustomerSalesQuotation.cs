using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Inspire.Erp.Domain.Entities
{
    public partial class CustomerSalesQuotation
    {
        public long CustomerQuotationQid { get; set; }
        public string CustomerQuotationVoucherNo { get; set; }
        public DateTime? customerQuotationQuotationdate { get; set; }
        public string CustomerQuotationDetailsCashCustomerName { get; set; }
        public decimal? CustomerQuotationDetailsQty { get; set; }
        public decimal? CustomerQuotationDetailsUnitPrice { get; set; }
     }
}
