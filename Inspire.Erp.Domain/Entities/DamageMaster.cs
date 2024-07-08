using System;
using System.Collections.Generic;

namespace Inspire.Erp.Domain.Entities
{
    public partial class DamageMaster
    {
        public int? DamageMasterId { get; set; }
        public DateTime? DamageMasterVdate { get; set; }
        public string DamageMasterNarration { get; set; }
        public int? DamageMasterLocationId { get; set; }
        public bool? DamageMasterDelStatus { get; set; }
        public string DamageMasterVoucherNumber { get; set; }
    }
    public class DamageEntryModel
    {
        public string LocationMasterLocationName { get; set; }
        public long? ItemMasterItemId { get; set; }
        public string ItemMasterItemName { get; set; }
        public string DamageDetailsVoucherNumber { get; set; }
        public long? StockRegisterMaterialId { get; set; }

        public int? DamageMasterID { get; set; }
        public DateTime? DamageMasterVdat { get; set; }
        public int? DamageMasterLocationID { get; set; }
        public string DamageMasterNarration { get; set; }
        public int? DamageDetailsMaterialID { get; set; }

        public double? DamageDetailsQTY { get; set; }

        public double? DamageDetailsPrice { get; set; }
        public double? DamageDetailsAmount { get; set; }
        public string? DamageDetailsRemarks { get; set; }
        public int? DamageDetailsUnitID { get; set; }
    }
    public class DamageEntryResponse
    {
        public string? Damage_Details_Voucher_Number { get; set; }
        public string? Location_Master_Location_Name { get; set; }
        public long? Item_Master_Item_ID { get; set; }
        public string? Item_Master_Item_Name { get; set; }
        public int? Stock_Register_Material_ID { get; set; }
        public int? Damage_Master_ID { get; set; }
        public DateTime? Damage_Master_Vdate { get; set; }
        public int? Damage_Master_Location_ID { get; set; }
        public string? Damage_Master_Narration { get; set; }
        public int? Damage_Details_Material_ID { get; set; }
        public double? Damage_Details_QTY { get; set; }
        public double? Damage_Details_Price { get; set; }
        public double? Damage_Details_Amount { get; set; }
        public string? Damage_Details_Remarks { get; set; }
        public int? Damage_Details_Unit_ID { get; set; }
    }
    public class DemageRequest
    {

        public List<VouchersNumbers> voucherNumberList { get; set; }
        public List<DamageMaster> damageMasterList { get; set; }
        public List<DamageDetails> damageDetailsList { get; set; }
        public List<StockRegister> stockRegisterList { get; set; }
    }
    public class DamageEntryViewModal
    {
        public string? Damage_Details_Voucher_Number { get; set; }
        public string? Location_Master_Location_Name { get; set; }
        public long? Item_Master_Item_ID { get; set; }
        public string? Item_Master_Item_Name { get; set; }
        public int? Stock_Register_Material_ID { get; set; }
        public int? Damage_Master_ID { get; set; }
        public DateTime? Damage_Master_Vdate { get; set; }
        public int? Damage_Master_Location_ID { get; set; }
        public string? Damage_Master_Narration { get; set; }
        public int? Damage_Details_Material_ID { get; set; }
        public double? Damage_Details_QTY { get; set; }
        public double? Damage_Details_Price { get; set; }
        public double? Damage_Details_Amount { get; set; }
        public string? Damage_Details_Remarks { get; set; }
        public int? Damage_Details_Unit_ID { get; set; }
    }
}
