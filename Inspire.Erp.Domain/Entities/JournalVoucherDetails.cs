using System;
using System.Collections.Generic;

namespace Inspire.Erp.Domain.Entities
{
    public partial class JournalVoucherDetails
    {
        //public long JournalVoucherDetails_ID { get; set; }
        //public long? JournalVoucher_ID { get; set; }
        //public string JournalVoucherDetails_VNO { get; set; }
        //public string JournalVoucherDetails_Acc_No { get; set; }
        //public double? JournalVoucherDetails_VatableAmount { get; set; }
        //public decimal? JournalVoucherDetails_Dr_Amount { get; set; }
        //public decimal? JournalVoucherDetails_FC_Dr_Amount { get; set; }
        //public decimal? JournalVoucherDetails_Cr_Amount { get; set; }
        //public decimal? JournalVoucherDetails_FC_Cr_Amount { get; set; }
        //public long? JournalVoucherDetails_JobNo { get; set; }
        //public int? JournalVoucherDetails_CostCenter_Id { get; set; }
        //public string JournalVoucherDetails_Narration { get; set; }
        //public string JournalVoucherDetails_Reference_No { get; set; }
        //public bool? JournalVoucherDetails_DelStatus { get; set; }
        ////public int JournalVoucherDetailsId { get; set; }
        ////public int JournalVoucherId { get; set; }
        ////public string JournalVoucherDetailsVno { get; set; }
        ////public string JournalVoucherDetailsAccNo { get; set; }
        ////public double? JournalVoucherDetailsVatableAmount { get; set; }
        ////public decimal? JournalVoucherDetailsDrAmount { get; set; }
        ////public decimal? JournalVoucherDetailsFcDrAmount { get; set; }
        ////public decimal? JournalVoucherDetailsCrAmount { get; set; }
        ////public decimal? JournalVoucherDetailsFcCrAmount { get; set; }
        ////public long? JournalVoucherDetailsJobNo { get; set; }
        ////public int? JournalVoucherDetailsCostCenterId { get; set; }
        ////public string JournalVoucherDetailsNarration { get; set; }
        ////public bool? JournalVoucherDetailsDelStatus { get; set; }
        ////public string JournalVoucherDetailsReferenceNo { get; set; }
        ///
        public int? JournalVoucherDetailsId { get; set; }
        public int? JournalVoucherDetailsAccSno { get; set; }
        public string JournalVoucherDetailVrefNo { get; set; }
        public string JournalVoucherDetailsAccNo { get; set; }
        public string JournalVoucherDetailsDrCr { get; set; }
        public double? JournalVoucherDetailsFcRate { get; set; }
        public double? JournalVoucherDetailsFcAmount { get; set; }
        public double? JournalVoucherDetailsAmount { get; set; }
        public string JournalVoucherDetailsNarration { get; set; }
        public int? JournalVoucherDetailsLocationId { get; set; }
        public string JournalVoucherDetailsReferenceNo { get; set; }
        public int? JournalVoucherDetailsJobId { get; set; }
        public int? JournalVoucherDetailsFsno { get; set; }
        public int? JournalVoucherDetailsSno { get; set; }
        public int? JournalVoucherDetailsCostCenterId { get; set; }
        public bool? JournalVoucherDetailsDelStatus { get; set; }

    }
}
