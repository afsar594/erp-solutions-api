using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.DTO.Asset_Register
{
    public class AssetRegisterDto
    {
        public int PurchaseNo { get; set; }
        public decimal AssetId { get; set; }
        public string Description { get; set; }
        public decimal DepreciationPercentage { get; set; }
        public string Supplier { get; set; }
        public decimal Amount { get; set; }
        public string AccumulatedDepreciation { get; set; }
        public decimal AccumulatedDepreciationPercentage { get; set; }
        public decimal BalanceDepreciationPercentage { get; set; }
        public decimal BookValue { get; set; }
        public string PurchaseDate { get; set; }
        public decimal DepPercBal { get; set; }
   
    }
}
    