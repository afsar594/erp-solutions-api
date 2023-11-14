using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Inspire.Erp.Domain.Entities
{
    public partial class PurchaseVoucher
    {
        public PurchaseVoucher()
        {
        }
        public long PurchaseVoucherPurID { get; set; }
        public string? PurchaseVoucherVoucherNo { get; set; }
        public int? PurchaseVoucherPurchaseIDNum { get; set; }
        public string? PurchaseVoucherPurchaseRef { get; set; }
        public string? PurchaseVoucherPurchaseType { get; set; }
        public string? PurchaseVoucherGRNo { get; set; }
        public DateTime? PurchaseVoucherGRDate { get; set; }
        public int? PurchaseVoucherSPID { get; set; }
        public string? PurchaseVoucherSPACCNO { get; set; }
        public decimal? PurchaseVoucherSPAmount { get; set; }
        public decimal? PurchaseVoucherFCSPAmount { get; set; }
        public DateTime? PurchaseVoucherPurchaseDate { get; set; }
        public string? PurchaseVoucherLPONo { get; set; }
        public DateTime? PurchaseVoucherLPODate { get; set; }
        public string? PurchaseVoucherQuatationNo { get; set; }
        public DateTime? PurchaseVoucherQuatationDate { get; set; }
        public decimal? PurchaseVoucherActualAmount { get; set; }
        public decimal? PurchaseVoucherNetAmount { get; set; }
        public decimal? PurchaseVoucherTransportCost { get; set; }
        public decimal? PurchaseVoucherHandlingCharges { get; set; }
        public decimal? PurchaseVoucherFCActualAmount { get; set; }
        public decimal? PurchaseVoucherFCNetAmount { get; set; }
        public string? PurchaseVoucherDescription { get; set; }
        public double? PurchaseVoucherDRACCNO { get; set; }
        public decimal? PurchaseVoucherDRAmount { get; set; }
        public decimal? PurchaseVoucherFCDRAmount { get; set; }
        public string? PurchaseVoucherDisYN { get; set; }
        public string? PurchaseVoucherDisACNo { get; set; }
        public decimal? PurchaseVoucherDisAmount { get; set; }
        public decimal? PurchaseVoucherFCDisAmount { get; set; }
        public string? PurchaseVoucherStatus { get; set; }
        public int? PurchaseVoucherFSNO { get; set; }
        public int? PurchaseVoucherUserID { get; set; }
        public decimal? PurchaseVoucherFCRate { get; set; }
        public int? PurchaseVoucherLocationID { get; set; }
        public string? PurchaseVoucherSupplyInvoiceNo { get; set; }
        public decimal? PurchaseVoucherDiscountPercentage { get; set; }
        public int? PurchaseVoucherPONo { get; set; }
        public int? PurchaseVoucherCurrencyID { get; set; }
        public int? PurchaseVoucherCompanyID { get; set; }
        public int? PurchaseVoucherJobID { get; set; }
        public string? PurchaseVoucherBatchCode { get; set; }
        public string? PurchaseVoucherCashSupplierName { get; set; }
        public string? PurchaseVoucherDayBookno { get; set; }
        public decimal? PurchaseVoucherVATAmount { get; set; }
        public decimal? PurchaseVoucherVATPercentage { get; set; }
        public string? PurchaseVoucherVATRoundSign { get; set; }
        public decimal? PurchaseVoucherVATRoundAmount { get; set; }
        public string? PurchaseVoucherVATNo { get; set; }
        public bool? PurchaseVoucherExcludeVAT { get; set; }
        public int? PurchaseVoucherIssuedID { get; set; }
        public bool? PurchaseVoucherJobDirectPurchase { get; set; }
        public string? PurchaseVoucherGRNOTMP { get; set; }
        public int? PurchaseVoucherPartyID { get; set; }
        public string? PurchaseVoucherPartyName { get; set; }
        public string? PurchaseVoucherPartyAddress { get; set; }
        public string? PurchaseVoucherPartyVatNo { get; set; }
        public decimal? PurchaseVoucherTotalGrossAmt { get; set; }
        public decimal? PurchaseVoucherTotalItemDisAmount { get; set; }
        public decimal? PurchaseVoucherTotalDiscountAmt { get; set; }
        public bool? PurchaseVoucherDelStatus { get; set; }
        //[NotMapped]
        //public virtual Suppliers PurchaseVoucherSp { get; set; }
        [NotMapped]
        public List<PurchaseVoucherDetails> PurchaseVoucherDetails { get; set; }
        [NotMapped]
        public List<AccountsTransactions> AccountsTransactions { get; set; }
        [NotMapped]
        public List<UserTrackingDisplay> UserTrackingData { get; set; }
    }
}
