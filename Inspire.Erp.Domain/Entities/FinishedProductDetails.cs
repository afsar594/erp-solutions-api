using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Inspire.Erp.Domain.Entities
{
    public class FinishedProductDetails
    {
        public decimal Finish_Product_DetailId { get; set; }
        public decimal? Finish_Product_Detail_Qty { get; set; }
        public decimal? Finish_ProductDetail_itemId { get; set; }
        public decimal? Finish_Product_Detail_UnitId { get; set; }
        public decimal? Finish_Product_Detail_Rate { get; set; }
        public decimal? Finish_Product_Detail_Amount { get; set; }
        public decimal? Finish_Product_MasterId { get; set; }
        public decimal? Finish_Product_Detail_Sno { get; set; }
        public decimal? Finish_Product_Detail_DelStatus { get; set; }

        public string? Finish_ProductDetail_VoucherNo { get; set; }
        public string? Finish_ProductDetail_PartNo { get; set; }


        public virtual FinishedProductMaster FinishedProductMaster { get; set; }

    }

    public class FinishedProductDetailsModel
    {
        public decimal Finish_Product_DetailId { get; set; }
        public decimal? Finish_Product_Detail_Qty { get; set; }
        public decimal? Finish_ProductDetail_itemId { get; set; }
        public decimal? Finish_Product_Detail_UnitId { get; set; }
        public decimal? Finish_Product_Detail_Rate { get; set; }
        public decimal? Finish_Product_Detail_Amount { get; set; }
        public decimal? Finish_Product_MasterId { get; set; }
        public decimal? Finish_Product_Detail_Sno { get; set; }
        public decimal? Finish_Product_Detail_DelStatus { get; set; }

        public string? Finish_ProductDetail_VoucherNo { get; set; }
        public string? Finish_ProductDetail_PartNo { get; set; }

    }
}
