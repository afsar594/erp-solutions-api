using System;
using System.Collections.Generic;

namespace Inspire.Erp.Domain.Entities
{
    public partial class StationMaster
    {
        public int? StationMasterCode { get; set; }
        public string StationMasterStationName { get; set; }
        public string StationMasterAddress { get; set; }
        public string StationMasterCity { get; set; }
        public string StationMasterPostOffice { get; set; }
        public string StationMasterTele1 { get; set; }
        public string StationMasterTele2 { get; set; }
        public string StationMasterFax { get; set; }
        public string StationMasterEmail { get; set; }
        public string StationMasterWebSite { get; set; }
        public string StationMasterCountry { get; set; }
        public string StationMasterLogoPath { get; set; }
        public string StationMasterSignPath { get; set; }
        public byte[] StationMasterLogoImg { get; set; }
        public byte[] StationMasterSignImg { get; set; }
        public byte[] StationMasterSealImg { get; set; }
        public string StationMasterVatNo { get; set; }
        public bool? StationMasterDelStatus { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
        public string IBAN { get; set; }
        public string SwiftCode { get; set; }
        public string BankBranch { get; set; }
        public string BankCurrency { get; set; }
       public string logotype { get; set; }
        
    }

    public partial class StationMasterList
    {
        public int? StationMasterCode { get; set; }
        public string StationMasterStationName { get; set; }
    }

    public partial class ReportName
    {
        public int id { get; set; }
        public int ClientCode { get; set; }
        public string DBName { get; set; }
        public string FormName { get; set; }
        public string ReportCaption { get; set; }
        public string ReprtFileName { get; set; }
        public bool ShowInForm { get; set; }
        public string SqlQuery { get; set; }
        public bool isactive { get; set; }
    }
}
