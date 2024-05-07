using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals
{
    public class WorkPeriodResponse
    {
        public int Id { get; set; }
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
        public int Quantity { get; set; }
        public double Total { get; set; }
        public double? BillAmount { get; set; }
        public double? CashAmount { get; set; }
        public double? ItemDiscount { get; set; }
        public double? BillDiscount { get; set; }
        public double? VAT { get; set; }

        public int VATRound { get; set; }
        public double? VATRoundAmount { get; set; }
        public double? DiscountVAT { get; set; }
        public string VATRoundSign { get; set; }
        public double? SalesTax { get; set; }

        public int UserId { get; set; }


    }

    public class SettlementDetailsResponse
    {
        public string TransactionDesc { get; set; }
        public int SettlementDetailsId { get; set; }
        public double? Amount { get; set; }
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
        public double? CardAmount { get; set; }
        public double? NetTotal { get; set; }
        public double? CashAmount { get; set; }
        public string RefereceNo { get; set; }
        public string VoucherNo { get; set; }
        public string CardType { get; set; }
        public int? UserId { get; set; }
        public int WorkPeriodId { get; set; }
        public int BillNoSetDet { get; set; }
        public long? BillnoLoad { get; set; }
        public double? BillAmount { get; set; }
        public double? ItemDiscount { get; set; }
        public double? BillDiscount { get; set; }
        public double? VAT { get; set; }
        public int VATRound { get; set; }
        public double? VATRoundAmount { get; set; }
        public double? DiscountVAT { get; set; }
        public string VATRoundSign { get; set; }
        public string PayMode { get; set; }
        public double? SalesTax { get; set; }
        public bool? SalesRet { get; set; }
        public string StationName { get; set; }
        public string CreditCardNumber { get; set; }
        public int? CustomerId { get; set; }
    }



}
