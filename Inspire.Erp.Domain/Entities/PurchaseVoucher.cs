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
    public int PurchaseVoucherPurID { get; set; }
    public string PurchaseVoucherVoucherNo { get; set; }
    public string? PurchaseVoucherPurchaseRef { get; set; }
    public string? PurchaseVoucherPurchaseType { get; set; }
    public string? PurchaseVoucherGRNo { get; set; }
    public DateTime? PurchaseVoucherGRDate { get; set; }
    public int? PurchaseVoucherSPID { get; set; }
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
    public string? PurchaseVoucherDisYN { get; set; }
    public decimal? PurchaseVoucherDisAmount { get; set; }
    public decimal? PurchaseVoucherFCDisAmount { get; set; }
    public string? PurchaseVoucherStatus { get; set; }
    public string? PurchaseVoucherSupplyInvoiceNo { get; set; }
    public decimal? PurchaseVoucherDiscountPercentage { get; set; }
    public string? PurchaseVoucherBatchCode { get; set; }
    public string? PurchaseVoucherCashSupplierName { get; set; }
    public bool? PurchaseVoucherJobDirectPurchase { get; set; }
    public string? PurchaseVoucherPartyName { get; set; }
    public string? PurchaseVoucherPartyAddress { get; set; }
    public string? PurchaseVoucherPartyVatNo { get; set; }
    public decimal? PurchaseVoucherTotalGrossAmt { get; set; }
    public decimal? PurchaseVoucherTotalItemDisAmount { get; set; }
    public decimal? PurchaseVoucherTotalDiscountAmt { get; set; }
    public bool? PurchaseVoucherDelStatus { get; set; }
    public int? PurchaseVoucherPurchaseIdNum { get; set; }
    public string PurchaseVoucherSpAccNo { get; set; }
    public decimal? PurchaseVoucherSpAmount { get; set; }
    public decimal? PurchaseVoucherFcSpAmount { get; set; }
    public double? PurchaseVoucherDrAccNo { get; set; }
    public decimal? PurchaseVoucherDrAmount { get; set; }
    public decimal? PurchaseVoucherFcDrAmount { get; set; }
    public string PurchaseVoucherDisAcNo { get; set; }
    public int? PurchaseVoucherFsno { get; set; }
    public int? PurchaseVoucherUserId { get; set; }
    public decimal? PurchaseVoucherFcRate { get; set; }
    public int? PurchaseVoucherLocationId { get; set; }
    public int? PurchaseVoucherPoNo { get; set; }
    public int? PurchaseVoucherCurrencyId { get; set; }
    public int? PurchaseVoucherCompanyId { get; set; }
    public int? PurchaseVoucherJobId { get; set; }
    public string PurchaseVoucherDayBookNo { get; set; }
    public decimal? PurchaseVoucherVatAmount { get; set; }
    public decimal? PurchaseVoucherVatPercentage { get; set; }
    public string PurchaseVoucherVatRoundSign { get; set; }
    public decimal? PurchaseVoucherVatRoundAmount { get; set; }
    public string PurchaseVoucherVatNo { get; set; }
    public bool? PurchaseVoucherExcludeVat { get; set; }
    public int? PurchaseVoucherIssuedId { get; set; }
    public string PurchaseVoucherGrnoTmp { get; set; }
    public int? PurchaseVoucherPartyId { get; set; }
    [NotMapped]
    public virtual Suppliers PurchaseVoucherSp { get; set; }
    [NotMapped]
    public virtual List<PurchaseVoucherDetails> PurchaseVoucherDetails { get; set; }
    [NotMapped]
    public virtual List<AccountsTransactions> AccountsTransactions { get; set; }
    [NotMapped]
    public List<UserTrackingDisplay> UserTrackingData { get; set; }
  }
}
