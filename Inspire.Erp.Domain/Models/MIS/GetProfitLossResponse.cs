using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals.MIS
{
   public class GetProfitLossResponse
    {
        public string head { get; set; }
        public string subHead { get; set; }
        public string accName { get; set; }
        public string debit { get; set; }
        public string credit { get; set; }
        public string amount { get; set; }
        public List<SubHeadProfitLoss> __children { get; set; }
    }
    public class SubHeadProfitLoss
    {
        public string accName { get; set; }
        public string subHead { get; set; }
        public string debit { get; set; }
        public string credit { get; set; }
        public string amount { get; set; }
        public List<AccNameProfitLoss> __children { get; set; }
    }
    public class AccNameProfitLoss
    {
        public string accName { get; set; }
        public string debit { get; set; }
        public string credit { get; set; }
        public string amount { get; set; }
    }
    public class ProfitLossWrapper
    {
        public List<GetProfitLossResponse> Details { get; set; }
        public string NetLoss { get; set; }
        public string TotalIncome { get; set; }
        public string TotalExpense { get; set; }
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
        public string StationMasterVatNo { get; set; }
    }
}
