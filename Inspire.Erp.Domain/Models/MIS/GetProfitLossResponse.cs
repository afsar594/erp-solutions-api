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
    public class GetProfitAndLossResponse
    {
        public string Head { get; set; }
        public string SubHead { get; set; }
        public string AccName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Amount { get; set; }
       
    }

    public class BalanceSheetTypeResponse
    {
        public string Head { get; set; }
        public string SubHead { get; set; }

        public string AccNo { get; set; }
        public string AccName { get; set; }
        public string RelativeNo { get; set; }
        public string RelativeName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Amount { get; set; }

    }


    public class ProfitAndLossWrapper
    {
        public List<GetProfitAndLossResponse> IncomeDetails { get; set; } = new();
        public List<GetProfitAndLossResponse> ExpenseDetails { get; set; } = new();

        public decimal TotalSalesCredit { get; set; }
        public decimal TotalSalesDebit { get; set; }
        public string SalesAccount { get; set; }
        public decimal TotalSalesReturnCredit { get; set; }
        public decimal TotalSalesReturnDebit { get; set; }

        public string TotalIncome { get; set; }
        public string TotalExpense { get; set; }
        public string NetProfit { get; set; }
        public string NetLoss { get; set; }

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
    public class ProfitAndLossDto
    {
        public string GroupKey { get; set; }             // yyyy-MM or yyyy-MM-dd depending on Mode
        public string HeadType { get; set; }             // INCOME, COGS, EXPENSES, OTHER INCOME, etc.
        public string SubHead { get; set; }              // Expense category, income type
        public string AccountName { get; set; }          // Account title
        public decimal TotalAmount { get; set; }         // Amount
        public string MonthName { get; set; }            // Only for Mode='MONTH'
        public string MonthKey { get; set; }             // 01–12
        public string FinalResult { get; set; }          // PROFIT / LOSS (from SP)
    }
    public class ProfitAndLossSummaryDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalCOGS { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalOtherIncome { get; set; }
        public decimal TotalOtherExpense { get; set; }
        public decimal NetProfit { get; set; }
        public string FinalResult { get; set; }        // PROFIT or LOSS
    }
    public class SpProfitAndLossResponse
    {
        public List<ProfitAndLossDto> Details { get; set; }
        public ProfitAndLossSummaryDto Summary { get; set; }
    }

}
