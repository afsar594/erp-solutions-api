using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Inspire.Erp.Domain.Entities
{
    public class FinishedProductMaster
    {
        public FinishedProductMaster()
        {
            FinishedProductDetails = new HashSet<FinishedProductDetails>();
        }

        public decimal Finish_Product_MasterId { get; set; }
        public string Finish_Product_MasterVoucherNo { get; set; }
        public DateTime? Finish_Product_MasterDate { get; set; }
        public decimal? Finish_Product_MasterLocationId { get; set; }
        public string Finish_Product_MasterNarration { get; set; }
        public string Finish_Product_MasterDelStatus { get; set; }

        [NotMapped]
        public virtual ICollection<FinishedProductDetails> FinishedProductDetails { get; set; }
    }

    public class FinishedProductMasterModel
    {

        public decimal Finish_Product_MasterId { get; set; }
        public string Finish_Product_MasterVoucherNo { get; set; }
        public DateTime? Finish_Product_MasterDate { get; set; }
        public decimal? Finish_Product_MasterLocationId { get; set; }
        public string Finish_Product_MasterNarration { get; set; }
        public string Finish_Product_MasterDelStatus { get; set; }
        public virtual List<FinishedProductDetails> FinishedProductDetails { get; set; }
    }
    public class FinishedProductDetailDto
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
