using Inspire.Erp.Domain.Modals.AccountStatement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals
{
    public class WorkPeriodResponse
    {
        public long Id { get; set; }
        public string WorkPeriodName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }
        public int? StationId { get; set; }
        public string StationName { get; set; }
        public int? UserId { get; set; }
        public int? UserEnd { get; set; }
        public decimal? OpeningCash { get; set; }
        public decimal? ClosingCash { get; set; }
        public string LoginSystemIP { get; set; }
        public string LoginComputerName { get; set; }
        public decimal? ClosingCashUser { get; set; }
        public int? UserClosebal { get; set; }
        public decimal? DifferenceAmount { get; set; }
        public decimal? OpeningCashUser { get; set; }
        public int? UserOpeningBal { get; set; }
    }

    public class SettlementDetailsRequest
    {
       // public int Quantity { get; set; }
        public decimal Total { get; set; }
        //public decimal? NetAmount { get; set; }
        public decimal? VAT { get; set; }
        public decimal? ItemDiscount { get; set; }
        public decimal? CashAmount { get; set; }
        public decimal? BillDiscount { get; set; }
        public decimal? BillDiscountPerc { get; set; }
        public decimal? SalesTax { get; set; }
        public int? VATRound { get; set; }
        public decimal? VATRoundAmount { get; set; }
        public decimal? DiscountVAT { get; set; }
        public string VATRoundSign { get; set; }
        public int UserId { get; set; }


    }
    public class CardAmountRequest
    {
        
        public decimal? Cash { get; set; }
        
        public int UserId { get; set; }


    }

    public class SettlementDetailsResponse
    {
        public string TransactionDesc { get; set; }
        public int SettlementDetailsId { get; set; }
        public decimal? Amount { get; set; }
        public long? SalesVoucherId { get; set; }
        public DateTime? SettlementDate { get; set; }
        public int? UserId { get; set; }
        public string  TransactionCode { get; set; }
        public int? SortOrder { get; set; }
        public bool? ShowinInvoice { get; set; }
        public string Status { get; set; }
    }
    public class CardAmountResponse
    {
        public decimal? NetTotal { get; set; }
        public decimal? CashAmount { get; set; }
        public decimal? Balance { get; set; }
        public decimal? LastBalance { get; set; }
    }


    public class CardRequest
    {
        public decimal? CardAmount { get; set; }
        public decimal? NetTotal { get; set; }
        public decimal? CashAmount { get; set; }
        public decimal? Cash { get; set; }
        public decimal? GrandTotal { get; set; }
        public string RefereceNo { get; set; }
        public string VoucherNo { get; set; }
        public string CardType { get; set; }
        public int? UserId { get; set; }
        public int WorkPeriodId { get; set; }
        public int BillNoSetDet { get; set; }
        public long? BillnoLoad { get; set; }
        public decimal? BillAmount { get; set; }
        public decimal? ItemDiscount { get; set; }
        public decimal? BillDiscount { get; set; }
        public decimal? BillDiscountPerc { get; set; }
        public decimal? VAT { get; set; }
        public int VATRound { get; set; }
        public decimal? VATRoundAmount { get; set; }
        public decimal? DiscountVAT { get; set; }
        public string VATRoundSign { get; set; }
        public string PayMode { get; set; }
        public decimal? SalesTax { get; set; }
        public bool? SalesRet { get; set; }
        public string StationName { get; set; }
        public string CreditCardNumber { get; set; }
        public int? CustomerId { get; set; }
        public int? SalesManId { get; set; }
        public int? LocationId { get; set; }
        public int? StationId { get; set; }

        public List<ItemDetail> ItemDetails { get; set; }
    }


    public class CardType
    {
        public int Id { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionDescription { get; set; }
        public bool? InHouse { get; set; }
       
    }

    public class ItemDetail
    {
        public int ItemId { get; set; }
        public string BarCode { get; set; }
        public string ItemName { get; set; }
        public string UnitName{ get; set; }
        public int UnitId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }

        public decimal? VatAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Total { get; set; }
        public decimal? NetTotal { get; set; }

        public int UnitDetailId { get; set; }

    }

}
