using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspire.Erp.Domain.Entities
{
    public partial class StockTransferDetails
    {
        public int StockTransferDetailsStId { get; set; }

        public int? StockTransferId { get; set; }
        public string StockTransferDetailsSNo { get; set; }
        public int? StockTransferDetailsMaterialId { get; set; }
        public int? StockTransferDetailsUnitId { get; set; }
        public double? StockTransferDetailsQty { get; set; }
        public double? StockTransferDetailsRate { get; set; }
        public string StockTransferDetailsRemarks { get; set; }
        public string StockTransferDetailsBatchCode { get; set; }
        public DateTime? StockTransferDetailsExpDate { get; set; }
        public bool? StockTransferDetailsDelStatus { get; set; }
        
        public decimal? StockTransferDetailsCostPrice { get; set; }
        [NotMapped]
        public virtual StockTransfer StockTransferDetailsNoNavigation { get; set; }
    }
}
