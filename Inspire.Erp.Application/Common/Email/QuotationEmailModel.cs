using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Common.Email
{
    public class QuotationEmailModel
    {
        public QuotationEmailModel()
        {
            Items = new List<QuotationItem>();
        }
        public int OrderId { get; set; }
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public string SenderName { get; set; }
        public string SenderPosition { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Year { get; set; }
        public string GrandTotal { get; set; }
        public List<QuotationItem> Items { get; set; }
    }
    public class QuotationItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Unit_Price { get; set; }
        public string Total { get; set; }
    }
}
