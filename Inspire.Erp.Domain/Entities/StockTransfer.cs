using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspire.Erp.Domain.Entities
{
    public partial class StockTransfer
    {
        public int? StockTransferId { get; set; }
        public string StockTransferVoucherNo { get; set; }
        public DateTime? StockTransferStDate { get; set; }
        public int? StockTransferLocationIdFrom { get; set; }
        public int? StockTransferLocationIdTo { get; set; }
        public int? StockTransferFSNo { get; set; }
        public string StockTransferStatus { get; set; }
        public int? StockTransferUserId { get; set; }
        public string StockTransferNarration { get; set; }
        public int? StockTransferJobId { get; set; }
        public bool? StockTransferApproved { get; set; }
        public string StockTransferApprovedBy { get; set; }
        public DateTime? StockTransferApprovedDate { get; set; }
        public bool? StockTransferDelStatus { get; set; }
        public decimal? StockTransferDepId { get; set; }
        public decimal? StockTransferCustomerId { get; set; }
        [NotMapped]
        public List<StockTransferDetails> StockTransferDetails { get; set; }
    }
}
