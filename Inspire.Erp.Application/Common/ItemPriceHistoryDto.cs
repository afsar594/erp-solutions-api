using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Common
{
    public class ItemPriceHistoryDto
    {
        public ItemPriceHistoryDto()
        {
            LastPurchasePrices = new List<PriceHistoryDto>();
            LastSalesPrices = new List<PriceHistoryDto>();
        }
        public long ItemId { get; set; }
        public string ItemName { get; set; }

        public List<PriceHistoryDto> LastPurchasePrices { get; set; }
        public List<PriceHistoryDto> LastSalesPrices { get; set; }
    }

    public class PriceHistoryDto
    {
        public decimal Price { get; set; }
        public string Date { get; set; }
    }
    public class ItemTransactionQueryResponse
    {
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
    }
}
