using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspire.Erp.Domain.Modals.Dashboard
{
    public class GetDashboardResponse
    {
        public decimal? Income { get; set; }
        public decimal? Expenses { get; set; }
        public decimal? Profit { get; set; }
        public DateTime? TransDate { get; set; }
        public string MonthYear { get; set; }
        public decimal? NetAmount { get; set; }
    }
    public class GetSalesResponse
    {
        public string MonthYear { get; set; }
        public decimal? NetAmount { get; set; }
    }
    public class GetSummaryResponse
    {
        public string Description { get; set; }
        public decimal Value { get; set; }
    }
    public class GetPDCResponse
    {
        public string VNO { get; set; }
        public string PartyName { get; set; }
        public string VType { get; set; }
        public string TDate { get; set; }
        public string PDate { get; set; }
        public string CNO { get; set; }
        public decimal CAmount { get; set; }
        public string CBName { get; set; }
        public string BAccNo { get; set; }
        public string AccName { get; set; }
        public string PID { get; set; }
    }
    public class GetCustomerSupplierResponse
    {
        public string MA_AccName { get; set; }
        public string MA_AccNo { get; set; }
        public decimal Amount { get; set; }
    }
    public class GetIncomeExpenseResponse
    {
        public decimal? Income { get; set; }
        public decimal? Expenses { get; set; }
        public decimal? Profit { get; set; }
        public DateTime? TransDate { get; set; }
    }
    public class PDCCountResponse
    {
        public int Total { get; set; }
        public int IssueRecieved { get; set; }
        public string Type { get; set; }
    }




    public class SalesPurchaseDashboardDto
    {
        public string Labels { get; set; }
        public string SalesData { get; set; }
        public string PurchaseData { get; set; }
        public ChartData ToChartData()
        {
            var labels = Labels?.Split(',') ?? Array.Empty<string>();
            var sales = SalesData?.Split(',').Select(s => decimal.TryParse(s, out var v) ? v : 0).ToList() ?? new List<decimal>();
            var purchase = PurchaseData?.Split(',').Select(s => decimal.TryParse(s, out var v) ? v : 0).ToList() ?? new List<decimal>();

            return new ChartData
            {
                Labels = labels.ToList(),
                Datasets = new List<ChartDataset>
                {
                    new ChartDataset { Label = "Sales", Data = sales },
                    new ChartDataset { Label = "Purchase", Data = purchase }
                }
            };
        }
    }
    public class ChartData
    {
        public List<string> Labels { get; set; } = new();
        public List<ChartDataset> Datasets { get; set; } = new();
    }
    public class ChartDataset
    {
        public string Label { get; set; }
        public List<decimal> Data { get; set; } = new();
    }


}
